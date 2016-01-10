// Write your Javascript code.

$(function () {
    $("input[type='date']").datepicker();

    //DisplayByExerciseInSelectForm();
});

/*
function DisplayByExerciseInSelectForm() {

    $("#BodyExerciseSelectList").find('option').each(function () {

        if($(this).val() != "0")
        {
            $(this).css("background-image", "url('/images/bodyexercises/" + $(this).val() + ".png')");
        }

        //alert($(this).val());
    });

    var component = $("BodyExerciseSelectList");
    
    if(component != null)
    {
        
        
    }
    
}*/

function displayBodyExerciseImage() {

    $("#BodyExerciseSelectList").find('option:selected').each(function () {
        $('#previewBodyExercise').hide();
        $('#previewBodyExercise').show(500);
        $('#previewBodyExercise').attr("src", "/images/bodyexercises/" + $(this).val() + ".png");
    });
}