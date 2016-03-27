/////////////////////////////////////////////////////////////////////////////////////
//  Utilitaires pour le site
//  Auteur: Nicolas Chourot
/////////////////////////////////////////////////////////////////////////////////////

$(".datepicker").datepicker({
    format: 'yyyy-mm-dd',
    orientation: "bottom auto"
});
$("#ImageUploader").change(function (e) { PreLoadImage(e); })
$("#UploadButton").click(function () { $("#ImageUploader").trigger("click"); })

function PreLoadImage(e) {
    // Saisir la référence sur l'image cible
    var imageTarget = $("#UploadedImage")[0];
    // Saisir la référence sur le fileUpload
    var input = $("#ImageUploader")[0];

    if (imageTarget != null) {
        var len = input.value.length;

        if (len != 0) {
            var fname = input.value;
            var ext = fname.substr(len - 3, len).toLowerCase();

            if ((ext != "png") &&
                (ext != "jpg") &&
                (ext != "bmp") &&
                (ext != "gif")) {
                bootbox.alert("Ce n'est pas un fichier d'image de format reconnu. Sélectionnez un autre fichier.");
            }
            else {
                var fReader = new FileReader();
                fReader.readAsDataURL(input.files[0]);
                fReader.onloadend = function (event) {
                    // event.target.result contiens les données de l'image
                    imageTarget.src = event.target.result;
                }
            }
        }
        else {
            imageTarget.src = null;
        }
    }
    return true;
}

$("a.confirm").click(function (e) {
    e.preventDefault(); // désactive la passation de l'événement
    var href = $(this).attr("href");
    var message = $(this).attr("message");
    bootbox.confirm(message, function (confirmResult) {
        if (confirmResult) {
            // demander au fureteur d'effectuer une requête vers l'url href
            window.location.href = href;
        }
    });
});

///////////////////////////////////////////////////////////////////
// On the click event on the image id="MoveLeft"
///////////////////////////////////////////////////////////////////
$("#MoveLeft").on('click', function () {
    $('#PoolList option:selected').each(function () {
        var movingItemVal = $(this).val();
        var movingItemText = $(this).text();
        if (movingItemText != "") {

            $('#PoolList option:selected').remove();
            var option = new Option(movingItemText, movingItemVal);
            $('#ItemList').append(option);
            SortSelect("ItemList");
            //DisableIcon("#MoveLeft");
        }
    });
})

///////////////////////////////////////////////////////////////////
// On the click event on the image id="MoveRight"
///////////////////////////////////////////////////////////////////
$("#MoveRight").on('click', function () {
    $('#ItemList option:selected').each(function () {
        var movingItemVal = $(this).val();
        var movingItemText = $(this).text();
        if (movingItemText != "") {

            $('#ItemList option:selected').remove();
            var option = new Option(movingItemText, movingItemVal);
            $('#PoolList').append(option);
            SortSelect("PoolList");
            //DisableIcon("#MoveRight");
        }
    });
})

///////////////////////////////////////////////////////////////////
// Save the items list id into the hidden input Items
// Note: The form submit button must have id="save"
///////////////////////////////////////////////////////////////////
$("#save").on('click', function (event) {
    var items_Id_String_List = "";
    $('#ItemList option').each(function () {
        items_Id_String_List += this.value + ",";
    });
    $("#Items").val(items_Id_String_List);
});

///////////////////////////////////////////////////////////////////
// Sort text items of a listbox
///////////////////////////////////////////////////////////////////
function SortSelect(selectId) {
    $("#" + selectId).html($("#" + selectId + " option").sort(function (a, b) {
        return a.text == b.text ? 0 : a.text < b.text ? -1 : 1;
    }))
}

