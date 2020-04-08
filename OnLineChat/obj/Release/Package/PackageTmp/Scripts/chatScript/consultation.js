
//界面初始化
$(document).ready(function () {

    //根据ID，获取问题列表
    $.ajax({
        async: true,
        url: "/OnLineChat/Consultation/getConsultation",
        type: "POST",
        data: {
            id: $("#current_id").val()
        },
        success: function (rJson) {
            var revice_data_array = JSON.parse(rJson);

            if (revice_data_array.result)
            {
                var contain_ul = $("#consultation_list_ul");

                for (var i = 0; i < revice_data_array.data.length; i++) {
                    var new_li = $("<li></li>");
                    new_li.attr("consultationid", revice_data_array.data[i].consultationid);
                    new_li.addClass("my_fav_list_li");

                    //<a  class="my_fav_list_a" href="" target="_blank">

                    var detali_a = $("<a></a>");
                    detali_a.addClass("my_fav_list_a");
                    detali_a.text(revice_data_array.data[i].consultationtitle);

                    new_li.append(detali_a)
                    contain_ul.append(new_li);
                }
            } else {
                alert(revice_data_array.msg);
            }
        }
    });

    //li的单击事件
    $('#consultation_list_ul').on('click', 'li', function (e) {
        var the_selected_id = $(this).attr("consultationid");

        $.ajax({
            async: true,
            url: "/OnLineChat/Consultation/getConsultationDetail",
            type: "POST",
            data: {
                id: $("#current_id").val(),
                consultationid: the_selected_id
            },
            success: function (rJson) {

                var revice_data_array = JSON.parse(rJson);
                if (revice_data_array.result)
                {
                    //consultationid: $("#consultation_id").val();
                    $("#input_title").val(revice_data_array.data[0].consultationtitle);
                    content: $("#input_content").val(revice_data_array.data[0].consultationdetail);
                           // appendix: $("#file_list").text();
                    
                    $("#consultataion_diag_y").dialog("open");

                } else {
                    alert("Error");
                }

            }
        });


    });

    //初始化咨询按钮
    $("#consultation_btn").button().click(function (event) {
        $.ajax({
            async: true,
            url: "/OnLineChat/Consultation/getConsultationID",
            type: "POST",
            data: {
                id: $("#current_id").val()
            },
            success: function (rJson) {
                $("#consultation_id").val(rJson);
                $("#consultataion_diag_y").dialog("open");
            }
        });

    });

    //弹框大小
    $("#consultataion_diag_y").dialog({
        title:"新增咨询",
        autoOpen: false,
        width: 500
    });

    //上传附件的按钮
    $("#upload_img_btn").button().click(function (event) {
        $("#upload_div_diag").dialog("open");
    });


    //上传附件的dialog
    $("#upload_div_diag").dialog({
        title: "上传图片",
        autoOpen: false,
        width: 500,
        buttons: [
            {
                text: "确定",
                click: function () {
                    //上传文件
                    $('#file_upload').data('uploadifive').settings.formData = {
                        'id': $("#current_id").val(),
                        'consultationid': $("#consultation_id").val()
                    };
                    $('#file_upload').uploadifive('upload'); //上传
                }
            },
            {
                text: "取消",
                click: function () {
                    $(this).dialog("close");
                }
            }
        ]
    });

    //内容提交
    $("#consultation_commit").button().click(function (event) {

        var check_flag = true;

        //增加判断条件

        if (check_flag)
        {
            $.ajax({
                async: true,
                url: "/OnLineChat/Consultation/addConsultation",
                type: "POST",
                data: {
                    id: $("#current_id").val(),
                    consultationid: $("#consultation_id").val(),
                    title: $("#input_title").val(),
                    content: $("#input_content").val(),
                    appendix: $("#file_list").text()
                },
                success: function (rJson) {

                    var revice_data_array = JSON.parse(rJson);
                    if (revice_data_array.result) {
                        //清空界面
                        clearForm();
                        alert("保存成功！");
                    } else {
                        alert(revice_data_array.msg);
                    }

                }
            });
        }

    });

    //上传文件
    $('#file_upload').uploadifive({
        'auto': false,
        'fileObjName': 'fileData',
        'fileType': 'image/*',
        'queueID': 'queue',
        'uploadScript': '/OnLineChat/Consultation/upLoad',
        'method': 'post',
        'buttonText': '添加图片',
        'formData': {
            'id': $("#current_id").val(),
            'consultationid': $("#consultation_id").val()
        },
        'uploadLimit': 0,
        'queueSizeLimit': 0,
        'onUploadComplete': function (file, data) {
            var revice_data_array = JSON.parse(data);

            if (revice_data_array.result) 
            {
                var temp_file_name = $("#file_name_list").html();
                var tempList = $("#file_list").text();
                if (tempList.length === 0) 
                {
                    temp_file_name = revice_data_array.appendfilename;
                    tempList = revice_data_array.appendmsg;
                } else {
                    temp_file_name = temp_file_name + "<br/>" + revice_data_array.appendfilename;
                    tempList = tempList + "^" + revice_data_array.appendmsg;
                }
                $("#file_list").text(tempList);
                $("#file_name_list").html(temp_file_name);
            } else {
                alert(revice_data_array.msg);
            }
        },
        'onQueueComplete': function (uploads) {
            //$("#upload_div_diag").dialog("close");
        }
    });

});

function clearForm()
{
    $("#consultation_id").val();
    $("#input_title").val();
    $("#input_content").val();
}