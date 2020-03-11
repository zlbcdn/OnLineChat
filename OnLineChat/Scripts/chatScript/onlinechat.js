var v_patient_id,
    v_patient_name,
    v_doctor_id,
    v_doctor_name,
    v_doctor_dept;

var visit_token;
var connect_flag = false;

$(document).ready(function () {

    //向医生咨询按钮
    $('#consultation_btn').button({
        icons: {
            primary: "ui-icon-mail-closed"
        }
    }).click(function () {

        //TO-DO：此部分需要完善，需要通过父界面传递患者基本信息。交界面
        //弹出对话框，录入患者及医生信息
        $("#patient_dialog").dialog({
            height: 200,
            width: 350,
            modal: true,
            buttons: [
                {
                    text: "确定",
                    click: function () {

                        //根据患者就诊基本信息，获取Token（有一定有效期的Token）
                        $.ajax({
                            async: true,
                            url: "/OnLineChat/Patient/getToken",
                            type: "POST",
                            data: {
                                patient_id: "10002018",
                                visit_date: "2020/3/1",
                                visit_dept: "1A000",
                                doctor_id:"A00239"
                            },
                            success: function (rJson) {

                                //授予唯一Token
                                visit_token = rJson;

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
                                if (connect_flag == false) {
                                    connectionService(v_patient_id, v_doctor_id);
                                }

                                //打开对话界面
                                $('.dialogue-main').css({ 'display': 'inline-block', 'height': '0' });
                                $('.dialogue-main').animate({ 'height': '600px' });

                                //关闭对话框
                                $("#patient_dialog").dialog("close");
                            }

                        });
                    }
                }]
        });// the end of dialog






        });//the end of consultation button 

    //关闭对话框
    $("#btn_close").click(function (e) {
        $('.dialogue-main').animate({ 'height': '0' }, function () {
            $('.dialogue-main').css({ 'display': 'none' });
        });
    });

    //发送的方法
    $("#dialogue_input").keydown(function (e) {
        sendMessage(e);
    });

    //清空发送框
    $("#dialogue_input").keyup(function (e) {
        var e = e || window.event;
        if (e.keyCode == 13) {
            document.getElementById("dialogue_input").value = "";
        }
    });


    $("#inputImage").change(function (e) {
        var file = this.files[0];
        if (undefined == file) {
            return;
        }
        var r = new FileReader();
        r.readAsDataURL(file);
        r.onload = function (e) {
            var base64 = e.target.result;

            sendMessageInfo("normal", v_patient_id, v_doctor_id, base64);
        }
    });

});//the end of init


//连接的方法
function connectionService(v_patient_id, v_doctor_id) {


    var wsImpl = window.WebSocket || window.MozWebSocket;

    //创建websocket
    window.ws = new wsImpl('ws://127.0.0.1:7181/');

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
    var e = e || window.event;

    //回车键
    if (e.keyCode == 13) {
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

    if (revice_data["message_from"] == v_patient_id) {
        nodeP.classList.add('dialogue-customer-contain');
        nodeSpan.classList.add('dialogue-text', 'dialogue-customer-text');
    } else {	
        nodeP.classList.add('dialogue-service-contain');
        nodeSpan.classList.add('dialogue-text', 'dialogue-service-text');
    }

    var revice_content = "";
    revice_content = revice_data["message_content"];
    if (revice_content.indexOf("base64") != -1 && revice_content.indexOf("data") != -1) {
        var nodeImg = document.createElement('img');
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
