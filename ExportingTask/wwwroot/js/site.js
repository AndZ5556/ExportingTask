document.onreadystatechange = function () {
    document.getElementById("ref").onclick = function (event) {
        event.preventDefault();
        newWindowPrinting();
    }
}

function newWindowPrinting() {
    window.open('about:blank', 'print_popup');
    $("#printForm").attr('target', 'print_popup');
    $("#printForm").submit();
    $("#printForm").attr('target', 'frame');
}

function beginPrint() {
    $("#waiting").css('display', 'block');
    StartLoadingFile('print', function () {
        $("#printForm").submit();
        $("#waiting").css('display', 'none');
        $("#popupPrint").css('display', 'block');
        if (window.chrome) {
            var frame = document.getElementsByName("frame")[0].contentWindow;
            var interval = setInterval(function () {
                if (frame.document && frame.document.contentType === 'application/pdf') {
                    console.log("Printed");
                    frame.print();
                    clearInterval(interval);
                }
            }, 100);
        } 
    })
}

function beginDownload() {
    $("#waiting").css('display', 'block');
    StartLoadingFile('download', function () {
        $("#waiting").css('display', 'none');
        $("#popupDownload").css('display', 'block');
    })
}

function StartLoadingFile(mode, callback) {
    $.ajax({
        method: "POST",
        url: "/Home/BeginLoadingFile",
        data: { mode: mode }
    }).done(function (res) {
        TryGetFile(callback);
    });
}

function TryGetFile(callback) {
    $.ajax({
        method: "POST",
        url: "/Home/GetFileTaskStatus",
    }).done(function (res) {
        if (res['success']) {
            callback();
        } else {
            setTimeout(function () { TryGetFile(callback) }, 100);
        }
    });
}
