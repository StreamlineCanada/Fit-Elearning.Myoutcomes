$(document).ready(function () {


    
    $(".text_question").blur(function () {

        var item = jQuery(this);

        if (item.hasClass("last"))
        {
            return;
        }
        var extra = item.data("extra");
        if (extra == "ORS")
        {
            var ors = parseInt(item.val());

            if (isNaN(ors) )
            {
                var qid = item.data("questionid");
                $("#saveStatus_" + qid).html("Enter a numeric value");
                item.val("");
                return;
            }
        }
        if (item.val() != "") {
            FitElearn.UserData.saveUserResponse(item);
        }

    });

    $(".radio_question").click(function () {

        var item = jQuery(this);        
        if (item.val() != "") {
            FitElearn.UserData.saveUserResponse(item);
        }

    });

    $(".radio_non_quiz_question").click(function () {

        var item = jQuery(this);
        if (item.val() != "") {
            FitElearn.UserData.saveUserResponse(item);
        }

    });

    $(".checkbox_question").click(function () {

        var item = jQuery(this);
        if (item.val() != "") {

            if (!item.attr('checked')) {
                item.val(0);
            }

            FitElearn.UserData.saveUserResponse(item);
        }

    });

    $("#confirmLessonComplete").click(function () {
        debugger;
        var item = $("#confirmLessonComplete");
        if (item.is(':checked')) {
            FitElearn.UserData.saveLessonComplete(item);
        }


    });

    $(".activity_text").blur(function () {

        var item = jQuery(this);
        if (item.val() != "") {
            FitElearn.UserData.saveUserActivity(item);
        }

    });

    $(".select_question").change(function () {

        var item = jQuery(this);
        if (item.val() != "") {
            FitElearn.UserData.saveUserResponse(item);
        }

    });

    $('textarea.text_question').keyup(function () {

        var item = jQuery(this);
        var text = item.val();
        if (text.length > 0)
        {
            var qid = item.data("questionid");
            $("#textarea_cc_" + qid).html(text.length + " out of 1200 characters max");

            if (text.length > 1200)
            {
                item.addClass("red");
                item.val(item.val().substring(0, 1200));
            }
            else
            {
                item.removeClass("red");

            }
        }

        

    });

});

