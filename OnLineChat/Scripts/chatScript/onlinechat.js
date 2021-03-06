﻿var v_patient_id,
    v_patient_name,
    v_doctor_id,
    v_doctor_name,
    v_doctor_dept;

var visit_token;
var connect_flag = false;

$(document).ready(function () {
            //患者基本信息
            v_patient_id = $("#patient_id_text").val();
            v_patient_name = $("#patient_name_text").val();
            v_doctor_id = $("#doctor_id_text").val();
            v_doctor_name = $("#doctor_name_text").val();
            v_doctor_dept = $("#doctor_dept_text").val();

            //设置头像、科室、医院等基本信息
            var doctor_pic_text = v_doctor_name.substr(0, 1);
            $("#doctor_pic").text(doctor_pic_text);
            $("#doctor_dept").text(v_doctor_dept);
            $("#hospital_name").text("北京清华长庚医院"); //需设定

            //服务器连接
    if (connect_flag === false) {
        if ("WebSocket" in window) { connectionService(v_patient_id, v_doctor_id); }
        else { alert("您好！浏览器不支持！请更换其他浏览器（例如：谷歌、搜狗等）"); }
    }

            //打开对话界面
            $('.dialogue-main').css({ 'display': 'inline-block', 'height': '0' });
            $('.dialogue-main').animate({ 'height': '600px' });


            //加载历史对话消息
        //getHistoryMessage();
    getHistoryMessage(v_patient_id, v_doctor_id);


    //发送的方法
    $("#dialogue_input").keydown(function (e) {
        sendMessage(e);
    });

    //清空发送框
    $("#dialogue_input").keyup(function (e) {
        var e1 = e || window.event;
        if (e1.keyCode === 13) {
            document.getElementById("dialogue_input").value = "";
        }
    });


    $("#inputImage").change(function (e) {
        var file = this.files[0];
        if (undefined === file) {
            return;
        }
        var r = new FileReader();
        r.readAsDataURL(file);
        r.onload = function (e) {
            var base64 = e.target.result;

            sendMessageInfo("normal", v_patient_id, v_doctor_id, base64);
        };
    });


    //查看图片的具体内容
    $("#dialogue_contain").on("click", 'img', function (e) {
        //打开一个Diag，将img数据填充
        var content_src= $(this).attr("src");

        $("#current_img").attr("src", content_src);
        $("#current_img").addClass("display-current-img");

        // max-width: 200px;

        var client_width = document.body.clientWidth;
        $("#current_img").css("max-width",client_width+"px");

        $("#current_img_div").dialog({
            height: 300,
            width: 400,
            modal: true
        }).dialog("open");// the end of dialog
    });



});//the end of init


//连接的方法
function connectionService(v_patient_id, v_doctor_id) {


    var wsImpl = window.WebSocket || window.MozWebSocket;

    //创建websocket
    window.ws = new wsImpl('ws://10.37.24.14:7181/');
    //window.ws = new wsImpl('ws://127.0.0.1:7181/');

    

    //the callback of open
    ws.onopen = function () {
        $("#connection_status").html('已连接');
        var id_json = {
            "message_type": "init",
            "message_from": v_patient_id,
            "message_to": v_doctor_id,
            "message_content": ""
        };

        var hello_message = JSON.stringify(id_json);
        ws.send(hello_message);

        //连接标志位
        connect_flag = true;
    };

    //关闭连接的回调方法
    ws.onclose = function () {
        connect_flag = false;
        $("#connection_status").html('未连接');
    };

    //当收到服务器的数据时的处理方法
    ws.onmessage = function (evt) {
        getServiceText(evt.data);
    };

    //the callback method of onerror
    ws.onerror = function (evt) {
        connect_flag = false;
    };
}

//发送数据
function sendMessage(e) {
    var e1 = e || window.event;

    //回车键
    if (e1.keyCode === 13) {
        sendMessageInfo("normal", v_patient_id, v_doctor_id, document.getElementById("dialogue_input").value);
    }

}

//发送消息的具体实现
function sendMessageInfo(v_message_type_arg,v_patient_id_arg,v_doctor_id_arg,v_message_content_arg) {
    var message_json = {
        "message_type": v_message_type_arg,
        "message_from": v_patient_id_arg,
        "message_to": v_doctor_id_arg,
        "message_content": v_message_content_arg
    };

    var content_message = JSON.stringify(message_json);
    window.ws.send(content_message);
}


//接收数据
function getServiceText(data) {

    var revice_data = JSON.parse(data);

    //通过追加P和Span实现
    var nodeP = document.createElement('p'),
        nodeSpan = document.createElement('span'),
        dialogueContain = document.getElementById('dialogue_contain');

    if (revice_data["message_from"] === v_patient_id) {
        nodeP.classList.add('dialogue-customer-contain');
        nodeSpan.classList.add('dialogue-text', 'dialogue-customer-text');
    } else {	
        nodeP.classList.add('dialogue-service-contain');
        nodeSpan.classList.add('dialogue-text', 'dialogue-service-text');
    }

    var revice_content = "";
    revice_content = revice_data["message_content"];
    if (revice_content.indexOf("base64") !== -1 && revice_content.indexOf("data") !== -1) {
        var nodeImg = document.createElement('img');
        nodeImg.classList.add("dialogue-img");
        nodeImg.src = revice_content;
        nodeSpan.appendChild(nodeImg);
    } else {
        nodeSpan.innerHTML = revice_content;
    }
    //将内容添加到HTML中
    nodeP.appendChild(nodeSpan);
    dialogueContain.appendChild(nodeP);

    //设定滚动条
    dialogueContain.scrollTop = dialogueContain.scrollHeight;
}

function getHistoryMessage(message_from_id,message_to_id) {
    //根据患者就诊基本信息，获取Token（有一定有效期的Token）
    $.ajax({
        async: true,
        url: "/OnLineChat/Patient/getAllMessage",
        type: "POST",
        data: {
            from_id: message_from_id,
            to_id: message_to_id
        },
        success: function (rJson) {

            var revice_data = JSON.parse(rJson);

            for (var i = 0; i < revice_data.length; i++)
            {
                addHistory(message_from_id, revice_data[i]);
            }

            var dialogueContain = document.getElementById('dialogue_contain');
            dialogueContain.scrollTop = dialogueContain.scrollHeight;
        }

    });

}


function addHistory(message_from_id, message_array) {
    //通过追加P和Span实现
    var nodeP = document.createElement('p'),
        nodeSpan = document.createElement('span'),
        dialogueContain = document.getElementById('dialogue_contain');

    if (message_array["message_from"] === message_from_id) {
        nodeP.classList.add('dialogue-customer-contain');
        nodeSpan.classList.add('dialogue-text', 'dialogue-customer-text');
    } else {
        nodeP.classList.add('dialogue-service-contain');
        nodeSpan.classList.add('dialogue-text', 'dialogue-service-text');
    }

    var revice_content = "";
    revice_content = message_array["message_content"];
    if (revice_content.indexOf("base64") !== -1 && revice_content.indexOf("data") !== -1) {
        var nodeImg = document.createElement('img');
        nodeImg.classList.add("dialogue-img");
        nodeImg.src = revice_content;
        nodeSpan.appendChild(nodeImg);
    } else {
        nodeSpan.innerHTML = revice_content;
    }
    //将内容添加到HTML中
    nodeP.appendChild(nodeSpan);
    dialogueContain.appendChild(nodeP);    
}



