if (window.jQuery) {

    var FitElearn = {};    

    FitElearn.Utilities = {

        ajax: function (fnName, postData) {

            return $.ajax({
                type: "GET",
                url: fnName,
                cache: false,
                data: postData,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                error: function (xhr, ajaxOptions, status) {

                    //alert('Reponse not saved. Please try again. ' + status);
                }
            });

        },
        verifyEmail: function () {

            $("#forgotBtn").hide();
            var email = $("#email").val();

            if (email == '')
            {
                $("#pw_msg").html("Please enter an email address");
                $("#pw_msg").addClass("red");
                $("#pw_msg").removeClass("green");

                $("#forgotBtn").show();
                return;
            }

            var lm = "email=" + email;            
            
            var result = FitElearn.Utilities.ajax("VerifyAndSendNewPassword", lm);
            
            result.then(function (response) {
                $("#pw_msg").html(response.message);
                if (response.success)
                {
                    $("#pw_msg").addClass("green");
                    $("#pw_msg").removeClass("red");
                }
                else {
                    $("#pw_msg").addClass("red");
                    $("#pw_msg").removeClass("green");
                }
            });

            $("#forgotBtn").show();
        }
        , changeProfile: function () {

            var userFields = new Array("username", "name", "email");
            var displayFields = new Array("Username", "Name", "Email");

            var errors = "";
            for(i=0;i<userFields.length;i++)
            {
                if ($("#"+userFields[i]).val() == "")
                {
                    errors += "Missing value for " + displayFields[i] + ".\r\n";
                }
            }

            if (!FitElearn.Utilities.validateEmail($("#email").val())) {
                errors += "Please enter a valid email address.\r\n";
            }

            if ($("#pw_section").is(":visible")) {
                if ($("#password").val() != $("#change_password").val()) {
                    errors += "Please make sure confirm password matches password.\r\n";
                }

                if ($("#password").val().length < 6)
                {
                    errors += "Password must be at least 6 characters long.\r\n";
                }
            }

            if (errors)
            {
                alert(errors);
                return;
            }

            var um = "email=" + $("#email").val() + "&name=" + $("#name").val() + "&id=" + $("#userid").val() + "&username=" + $("#username").val();
            if ($("#pw_section").is(":visible")) {
                um += "&password=" + $("#password").val();
            }
            else {
                um += "&password=";
            }

            var result = FitElearn.Utilities.ajax("UpdateProfile", um);

            result.then(function (response) {
                
                if (response.success) {
                    $("#profileMsg").html("Update Successful!");
                    $("#profileMsg").addClass("green");
                    $("#profileMsg").removeClass("red");
                }
                else {
                    $("#profileMsg").html(response.errorMsg);
                    $("#profileMsg").addClass("red");
                    $("#profileMsg").removeClass("green");
                }
            });

        }
        , showPassword: function () {
            $("#pw_section").show();
        }
        , validateEmail: function (email) {
              var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
                if( !emailReg.test( email ) ) {
                    return false;
                } else {
                    return true;
                }
        }
        , createUser: function () {

            var userFields = new Array("loginuserid", "name", "email");
            var displayFields = new Array("Login User ID", "Name", "Email");

            var errors = "";
            
            for (i = 0; i < userFields.length; i++) {
                if ($("#" + userFields[i]).val() == "") {
                    errors += "Missing value for " + displayFields[i] + ".\r\n";
                }
            }

            if (!FitElearn.Utilities.validateEmail($("#email").val())) {
                errors += "Please enter a valid email address.\r\n";
            }

            if (errors) {
                alert(errors);
                return;
            }

            var basicMod, advancedMod = 0;
            if ($("#basicmodule").is(':checked')) {
                basicMod = 1;
            }
            if ($("#advancedmodule").is(':checked')) {
                advancedMod = 1;
            }

            var um = "LoginUserId=" + $("#loginuserid").val() + "&Name=" + $("#name").val() + "&Email=" + $("#email").val() + "&CompanyName=" + $("#companyname").val() + "&FitNumber=" + $("#fitnumber").val() + "&BasicModule=" + basicMod + "&AdvancedModule=" + advancedMod;

            var result = FitElearn.Utilities.ajax("CreateUser", um);

            result.then(function (response) {

                if (response.Success) {
                    alert('Success');
                    $("#profileMsg").html("Successfully created new user !");
                    $("#profileMsg").addClass("green");
                    $("#profileMsg").removeClass("red");
                }
                else {
                    alert('Failure');
                    $("#profileMsg").html(response.errorMsg);
                    $("#profileMsg").addClass("red");
                    $("#profileMsg").removeClass("green");
                }
            });

        }
        , updateUser: function () {

            var userFields = new Array("loginuserid", "name", "email");
            var displayFields = new Array("Login User ID", "Name", "Email");

            var errors = "";

            for (i = 0; i < userFields.length; i++) {
                if ($("#" + userFields[i]).val() == "") {
                    errors += "Missing value for " + displayFields[i] + ".\r\n";
                }
            }

            if (!FitElearn.Utilities.validateEmail($("#email").val())) {
                errors += "Please enter a valid email address.\r\n";
            }

            if (errors) {
                alert(errors);
                return;
            }

            var basicMod, advancedMod = 0;
            if ($("#basicmodule").is(':checked')) {
                basicMod = 1;
            }
            if ($("#advancedmodule").is(':checked')) {
                advancedMod = 1;
            }

            var um = "Id=" + $("#userid").val() + "&LoginUserId=" + $("#loginuserid").val() + "&Name=" + $("#name").val() + "&Email=" + $("#email").val() + "&CompanyName=" + $("#companyname").val() + "&FitNumber=" + $("#fitnumber").val() + "&BasicModule=" + basicMod + "&AdvancedModule=" + advancedMod;

            var result = FitElearn.Utilities.ajax("UpdateUser", um);

            result.then(function (response) {

                if (response.Success) {
                    alert('Success');
                    $("#profileMsg").html("User updated successfully !");
                    $("#profileMsg").addClass("green");
                    $("#profileMsg").removeClass("red");
                }
                else {
                    alert('Failure');
                    $("#profileMsg").html("error");
                    $("#profileMsg").addClass("red");
                    $("#profileMsg").removeClass("green");
                }
            });

        }

        , deactivateUser: function (id, loginuserid) {            
            if (confirm("Are you sure you want to deactivate this stand-alone user. Click 'Ok' to procced")) {
                var um = "Id=" + id + "&LoginUserId=" + loginuserid;
                var result = FitElearn.Utilities.ajax("DeactivateUser",um);

                result.then(function (response) {                    
                    if (response === 1) {
                        alert('Success');
                        $("#profileMsg").html("User with UserId: " + loginuserid + " deactivated successfully!");
                        $("#profileMsg").addClass("green");
                        $("#profileMsg").removeClass("red");
                        location.reload();
                    }
                    else {
                        alert('Failure');
                        $("#profileMsg").html("Error occured");
                        $("#profileMsg").addClass("red");
                        $("#profileMsg").removeClass("green");
                    }                    
                });
            }            
        }
        , reactivateUser: function (id, loginuserid) {
            if (confirm("Are you sure you want to reactivate this stand-alone user. Click 'Ok' to procced")) {
                var um = "Id=" + id + "&LoginUserId=" + loginuserid;
                var result = FitElearn.Utilities.ajax("ReactivateUser", um);

                result.then(function (response) {
                    if (response === 1) {
                        alert('Success');
                        $("#profileMsg").html("User with UserId: " + loginuserid + " reactivated successfully!");
                        $("#profileMsg").addClass("green");
                        $("#profileMsg").removeClass("red");
                        location.reload();
                    }
                    else {
                        alert('Failure');
                        $("#profileMsg").html("Error occured");
                        $("#profileMsg").addClass("red");
                        $("#profileMsg").removeClass("green");
                    }
                });
            }
        }
    };

    FitElearn.UserData = {

        saveUserResponse: function (item) {

            var moduleId = $("#moduleId").val();

            var questionId = item.data("questionid");
  
            var questionTypeId = item.data("questiontypeid");
            var lessonId = item.data("lessonid");

            if (item.hasClass("slider_question"))
            {
                var response = item.slider("option", "value");
            }
            else {
                var response = escape(item.val());
            }

            console.log(response);

            var statusElem = $("#saveStatus_" + questionId);            
            var ur = "moduleId=" + moduleId + "&lessonId=" + lessonId + "&questionId=" + questionId + "&questionTypeId=" + questionTypeId + "&response=" + response;
            var numRadios = parseFloat($(":radio[name='question_" + questionId + "']").length);
            
            statusElem.html("Saving...");

            var valueSet = response;

            var result = FitElearn.Utilities.ajax("SaveUserResponse", ur);
            console.log(result);
            result.then(function (response) {
                if (Number(response.error) > 0) {
                    statusElem.html("An error occured saving this response. Please try again.");
                    //answerStatus.html("");
                    $("#nextBtn").show();
                }
                else {
                    if (Number(response.userResponseId) > 0) {
                        statusElem.html("Saved");
                    }
                    else {
                        statusElem.html("Updated");
                    }

                    if (item.hasClass("last"))
                    {
                        var nextPageUrl = $("#nextPageUrl").val();
                        window.location.href = nextPageUrl;
                        return 0;
                    }
                    else
                    {

                        $(".answerStatus_" + questionId).html("");


                        if (item.hasClass("radio_question")) {

                            var answerStatus = $("#answerStatus_" + valueSet);

                            if (Number(response.correct) == 1) {

                                if (questionTypeId == 1) {

                                    var questionScore = $("#score_" + questionId);

                                    var score = parseFloat(questionScore.val());
                                    score = score + 1;
                                    if (score > 1) {
                                        score = 1;
                                    }
                                    questionScore.val(score);
                                }

                                $("#answered_locked_" + questionId).val(1);
                                $(":radio[name='question_" + questionId + "']").attr('disabled', true);
                                answerStatus.html("<img src='../Images/icons/correct_icon.png' alt='Correct' style='vertical-align:bottom' >");
                            }
                            else {

                                if (questionTypeId == 1) {
                                    var questionScore = $("#score_" + questionId);

                                    var score = parseFloat(questionScore.val());
                                    score = (score - (1 / numRadios)).toFixed(2);
                                    if (score < -1) {
                                        score = -1;
                                    }
                                    questionScore.val(score);
                                }

                                answerStatus.html("<img src='../Images/icons/incorrect_icon.png' alt='Incorrect' style='vertical-align:bottom' >");
                            }
                        }
                    }
                    
                }
            });

        },

        saveUserActivity: function (item) {

            var moduleId = $("#moduleId").val();

            var activityInputId = item.data('activityinputid');
            var lessonId = item.data("lessonid");
            var activityText = escape(item.val());


            var ur = {
                moduleId: moduleId,
                lessonId: lessonId,
                activityInputId: activityInputId,
                activityText: activityText
            };

            var ur = "moduleId=" + moduleId + "&lessonId=" + lessonId + "&activityInputId=" + activityInputId + "&activityText=" + activityText;


            var statusElem = $("#saveStatus_activity_" + activityInputId);
            statusElem.html("Saving...");

            var result = FitElearn.Utilities.ajax("SaveUserActivity", ur);
            result.then(function (response) {
                if (Number(response.error) > 0) {
                    statusElem.html("An error occured saving this response. Please try again.");
                }
                else {
                    if (Number(response.userActivityId) > 0) {
                        statusElem.html("Saved");
                    }
                    else {
                        statusElem.html("Updated");
                    }
                }
            });

        },

        getUserActivity: function (item) {

            var moduleId = $("#moduleId").val();

            var activityInputId = item.data('activityinputid');
            var lessonId = item.data("lessonid");

            var ur = "moduleId=" + moduleId + "&lessonId=" + lessonId + "&activityInputId=" + activityInputId;

            var result = FitElearn.Utilities.ajax("GetUserActivity", ur);
            result.then(function (response) {

                item.val(response.activityText);


            });

        },


        saveLessonComplete: function () {



            var moduleId = $("#moduleId").val();
            var lessonId = $("#lessonId").val();

            var lcr = "moduleId=" + moduleId + "&lessonId=" + lessonId + "&lessonComplete=true";
            console.log(lcr);
            var statusElem = $("#saveStatus");
            statusElem.html("Saving...");

            var result = FitElearn.Utilities.ajax("SaveLessonComplete", lcr);
            result.then(function (response) {
                if (Number(response.error) > 0) {
                    statusElem.html("An error occured saving this response. Please try again.");
                }
                else {
                    statusElem.html("Response Saved");
                    $("#nextBtn").show();
                }
            });

        },

        saveQuizComplete: function (score_sum) {

            var moduleId = $("#moduleId").val();
            var lessonId = $("#lessonId").val();

            score_sum = "" + score_sum;

            var qcr = "moduleId=" + moduleId + "&lessonId=" + lessonId + "&score=" + score_sum;


            var statusElem = $("#saveStatus");
            statusElem.html("Saving...");

            var result = FitElearn.Utilities.ajax("SaveQuizComplete", qcr);


            result.then(function (response) {
                if (Number(response.error) > 0) {
                    statusElem.html("An error occured saving this response. Please try again.");
                }
                else {
                    statusElem.html("Saved");
                    window.location.href = "LessonFlowComplete?moduleId=" + moduleId + "&lessonId=" + lessonId;
                }
            });

        }
    };

    FitElearn.Question = {

        allAnswered: function () {

            var blank = false;
            $("input:radio").each(function () {
                var val = $('input:radio[name=' + this.name + ']:checked').val();
                if (val === undefined) {
                    blank = true;
                    return false;
                }
            });

            if (blank) {
                alert("Please answer all questions to continue.");
            }
            else {
                var sum = 0;
                $(".scores").each(function () {


                    var score_id = this.id;
                    var question_id = score_id.replace("score_", "");

                    var anwered_locked = $("#answered_locked_" + question_id).val();
                    if (anwered_locked == 1)
                    {
                        var score = parseFloat($("#" + this.id).val());

                        sum += score;
                    }

                });

                if (sum < 0) {
                    sum = 0;
                }

                FitElearn.UserData.saveQuizComplete(sum);
            }

        }
        , checkAllResponses: function (url) {

            $("#nextBtn").hide();

            var totalTextBoxes = $(".text_question").length;
            var totalResponses = 0;

            var tooLong = false;

            $(".text_question").each(function () {

                var val = $("#" + this.id).val();
                if (val != "") {
                    totalResponses++;

                    
                }
            });

            var errors = "";

            if (totalTextBoxes != totalResponses)
            {
                errors += "Please complete responses to all questions.\r\n";
                
            }

            if (errors != "")
            {
                alert(errors);
                $("#nextBtn").show();
                return;
            }

            var last = $(".last");
            if (last.val() != undefined)
            {
                FitElearn.UserData.saveUserResponse(last);
            }
            else
            {
                var nextPageUrl = $("#nextPageUrl").val();
                window.location.href = nextPageUrl;
                return 0;
            }
                
            
        }
    };

}



