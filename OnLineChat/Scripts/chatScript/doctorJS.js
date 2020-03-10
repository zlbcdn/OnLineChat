var connect_flag = false;
var v_doctor_id, v_patient_id;


//界面初始化
$(document).ready(function () {

    //医生登录
    $("#doctor_sign_in_btn").button().click(function () {
        $("#doctor_dialog").dialog({
            height: 200,
            width: 350,
            modal: true
        });
    });


    //获取医生登录信息
    $("#sign_in_btn").button().click(function () {

        v_doctor_id = $("#doctor_id_text").val();

        //$('.wrapper').css({ 'display': 'inline-block', 'height': '0' });
        //$('.wrapper').animate({ 'height': '800px' });

        $.ajax({
            async: true,
            url: "/OnLineChat/Doctor/getRegisterPatientList",
            type: "POST",
            data: {
                doctor_id: "10002018",
                dept_id: "2020/3/1",
                visit_date: "1A000",
                register_type: "A00239"
            },
            success: function (rJson) {

                var revice_data_array = JSON.parse(rJson);
                var the_people_continer = document.getElementById("the_people_container");

                for (var i = 0; i < revice_data_array.length; i++)
                    {
                        //创建存放列表
                        var liNode = document.createElement('li');
                        liNode.classList.add('person');
                        liNode.setAttribute("data-chat", revice_data_array[i].VisitID);

                        //设置头像
                        var img_node = document.createElement('img');
                        img_node.setAttribute("alt", "");
                        if (revice_data_array[i].Sex == "F") { img_node.src= "../Scripts/styles/icon_people/female.png"; }
                        else { img_node.src = "../Scripts/styles/icon_people/male.png"; }

                        //设置姓名
                        var span_name_node = document.createElement('span');
                        span_name_node.classList.add('name');
                        span_name_node.setAttribute("the_patient", revice_data_array[i].PatientID);
                        span_name_node.innerText = revice_data_array[i].PatientName;

                        //设置
                        var span_online_status_node = document.createElement('span');
                        span_online_status_node.setAttribute("the_patient", revice_data_array[i].PatientID);
                        span_online_status_node.classList.add('onlinestaus');
                        span_online_status_node.innerText = "";
                        

                        //设置时间
                        var span_time_node = document.createElement('span');
                        span_time_node.classList.add('time');
                        span_time_node.innerText = "1";

                        //设置预览
                        var span_preview_node = document.createElement('span');
                        span_preview_node.classList.add('preview');
                    span_preview_node.innerText = "";

                    //设置病历号
                    var span_patient_node = document.createElement('span');
                    span_patient_node.classList.add('hidden');
                    span_patient_node.setAttribute("the_patient", revice_data_array[i].PatientID);
                    span_patient_node.innerText = revice_data_array[i].PatientID;

                        liNode.appendChild(img_node);
                        liNode.appendChild(span_name_node);
                        liNode.appendChild(span_online_status_node);
                        liNode.appendChild(span_time_node);
                    liNode.appendChild(span_preview_node);
                    liNode.appendChild(span_patient_node);

                        the_people_continer.appendChild(liNode);
                    }

                //连接
                if (connect_flag == false) { connectionService(v_doctor_id);}

                $("#doctor_dialog").dialog("close");
                }
        });


        //为数据添加监听事件
        $('#the_people_container').on('click', 'li', function (e) {

            $("#the_people_container .active").removeClass("active"); //先取消当前的激活样式
            $(this).addClass("active"); //将激活样式“授予”当前的li元素

            //授予界面
            var select_name = $(this).find('span:first').html();
            $("#current_patient").text(select_name);

            //去掉之前的聊天内容
            $('.container .right').find('.active-chat').removeClass('active-chat');

            var data_chat= $(this).attr('data-chat');
            $('.container .right').find('[data-chat="' + data_chat+'"]').addClass('active-chat');

            v_patient_id = $(this).find('span:last').html();

            //发送消息
            var id_json = {
                "message_type": "init",
                "message_from": v_doctor_id,
                "message_to": v_patient_id,
                "message_content": ""
            };

            var hello_message = JSON.stringify(id_json);
            ws.send(hello_message);


        });





    });

    //发送的方法
    $("#send_message").keydown(function (e) {
        sendMessage(e);
    });

    //清空发送框
    $("#send_message").keyup(function (e) {
        var e = e || window.event;
        if (e.keyCode == 13) {
            document.getElementById("send_message").value = "";
        }
    });



});

//连接的方法
function connectionService(v_doctor_id) {


    var wsImpl = window.WebSocket || window.MozWebSocket;

    //创建websocket
    window.ws = new wsImpl('ws://127.0.0.1:7181/');

    //the callback of open
    ws.onopen = function () {
        $("#connection_status").html('已连接');
        var id_json = {
            "message_type": "init",
            "message_from": v_doctor_id,
            "message_to": "",
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
        var message_json = {
            "message_type": "normal",
            "message_from": v_doctor_id,
            "message_to": v_patient_id,
            "message_content": document.getElementById("send_message").value
        };

        var content_message = JSON.stringify(message_json);
        window.ws.send(content_message);
    }

}

//接收数据
function getServiceText(data) {

    var revice_data = JSON.parse(data);

    //初始化
    if (revice_data["message_type"] == "init")
    {
        $("#the_people_container span.onlinestaus[the_patient = '" + revice_data["message_from"] + "']").text("在线");
    }

    //关闭
    if (revice_data["message_type"] == "close") {
        $("#the_people_container span.onlinestaus[the_patient = '" + revice_data["message_from"] + "']").text("");
    }

    //正常数据
    if (revice_data["message_type"] == "normal")
    {
        var receive_message_from = revice_data["message_from"];
        var message_from_self = false;//消息是否为自己发送的标志位

        var temp_patient_id = receive_message_from;
        if (receive_message_from == v_doctor_id) {
            temp_patient_id = revice_data["message_to"];
            message_from_self = true;
        }

        var temp_data_chat= $("#the_people_container").find("[the_patient = '" + temp_patient_id + "']").parent().attr("data-chat");
    
    

        //获取数据
        var select_chat = $("div[data-chat='" + temp_data_chat + "']");


        if (!(select_chat.length && select_chat.length > 0)) { 
            var chat_div = document.createElement("div");
            chat_div.classList.add('chat');
            chat_div.setAttribute("data-chat", temp_data_chat);

            $(".wrapper .container .right .top").after(chat_div);
        }

        //再次确认并赋值
        select_chat = $("div[data-chat='" + temp_data_chat + "']");

        var message_div = $("<div></div>");
        if (message_from_self) { message_div.addClass("bubble me"); }
        else { message_div.addClass("bubble you"); }

        message_div.text(revice_data["message_content"]);

        select_chat.append(message_div);

        select_chat.scrollTop = $(".right").scrollHeight;
    }
    
}