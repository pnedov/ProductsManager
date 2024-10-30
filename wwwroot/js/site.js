// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function SaveEdit(id) {
    console.log(id);
    var row = document.querySelector(`tr[data-id='${id}']`);
    var reqItem = {
        "Id": id,
        "Name": row.querySelector("input[name='Name']").value,
        "Quantity": row.querySelector("input[name='Quantity']").value,
        "Price": row.querySelector("input[name='Price']").value,
        "UniqueCode": row.querySelector("input[name='UniqueCode']").value,
        "SuppliersId": row.querySelector("select[name='SuppliersId']").value,
        "Status": row.querySelector("select[name='Status']").value
    };

    $.ajax({
        url: "/warehouse/edit",
        type: "POST",
        data: JSON.stringify(reqItem),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var div_success = $('body').find("#msg-success");
            div_success.show();
            div_success.children('div').text(response.message);
            var tr = $('table tr[data-id="' + id + '"]');
            tr.addClass("blink");
            setTimeout(() => {
                tr.removeClass("blink");
            }, 2000);
        },
        error: function (data) {
            var div_error = $('body').find("#msg-error");
            div_error.show();
            div_error.children('div').text(data.message);
            console.log(data.message);
        }
    });
}

function ReloadCancel(id) {
    var row = $('table tr[data-id="' + id + '"]');
    $.ajax({
        url: "/warehouse/get-item?id=" + id,
        type: "GET",
        success: function (data) {
            $(row.find("input[name='Name']")).val(data.name);
            $(row.find("input[name='Quantity']")).val(data.quantity);
            $(row.find("input[name='Price']")).val(data.price);
            $(row.find("input[name='UniqueCode']")).val(data.uniqueCode);
            $(row.find("select[name='SuppliersId']")).val(data.suppliersId).change();
            $(row.find("select[name='Status']")).val(data.status).change();
        },
        error: function (data) {
            var div_error = $('body').find("#msg-error");
            div_error.show();
            div_error.children('div').text(data.message);
            console.log(data.message);
        }
    });
}

$("#addNewButton").click(function () {
    $("#addNewForm").toggle();
});

$(document).on('click', 'body', function () {
    $("#msg-success,#msg-error").hide();
});

$(document).on('click', '#refresh', function () {
    window.location.reload();
});

// * Multi delete functionalities * //

//list check all/none handler
$(document).on('click', '.on-list-chkall', function (e) {
    var $cbs = $(".multicb", this.form).prop('checked', this.checked);
    if (this.checked) {
        $cbs.closest("tr").addClass("selected");
    } else {
        $cbs.closest("tr").removeClass("selected");
    }
});

$(document).on('click', '.on-delete-multi', function (e) {
    var el = this;
    if (!el._is_confirmed) {
        e.preventDefault();
        if (!$('.multicb:checked').length) return;//exit if no rows selected
        if (confirm("Are you sure to delete multiple selected records?")) {
            el.closest("form").submit();
        } else {
            return false;
        }
    }
});

//make list multi buttons floating if at least one row checked
$(document).on('click', '.on-list-chkall, .multicb', function (e) {
    var $this = $(this);
    var $bm = $('#list-btn-multi');
    var len = $('.multicb:checked').length;
    if (len > 0) {
        //float
        $bm.addClass('position-sticky');
        $bm.find('.rows-num').text(len);
    } else {
        //de-float
        $bm.removeClass('position-sticky');
        $bm.find('.rows-num').text('');
    }
    if ($this.is(".multicb")) {
        if (this.checked) {
            $this.closest("tr").addClass("selected");
        } else {
            $this.closest("tr").removeClass("selected");
        }
    }

    //also fill item_id with comma-separated checked ids
    var ids = [];
    $('.multicb:checked').each(function (i, el) {
        var name = el.name.replace('cb[', '');
        name = name.replace(']', '');
        ids.push(name); //leave just id
    });
    if (ids.length) $('form[name="form_delete"]').find('input[name="cbitems"]').val(ids.join(','));
});


//filters auto-submit
$(document).on('change', '#formFilters', function (e) {
    $(this).closest('form').submit();
});

//cancel button
$(document).on('click', "input[name = 'cancel']", function (e) {
    e.preventDefault();
    $("#formIndex").submit();
});





