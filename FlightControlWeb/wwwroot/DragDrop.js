let dropArea = document.getElementById('myflights');
let dropAreaText = "";
let upload = false;

dropArea.addEventListener('dragenter', dragenter, false);
dropArea.addEventListener('dragleave', dragleave, false);
//dropArea.addEventListener('dragover', handlerFunction, false);
dropArea.addEventListener('drop', handleDrop, false);

;['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
    dropArea.addEventListener(eventName, preventDefaults, false);
})

function preventDefaults(e) {
    e.preventDefault();
    e.stopPropagation();
}

function dragenter() {
    if (!upload) {
        dropAreaText = dropArea.innerHTML;
        dropArea.innerHTML = '<div class="container" style="pointer-events: none;"> <form class="my-form"><p><b>Upload files by dragging them onto the dashed area</b><br /><span class="fa fa-cloud-upload-alt"></span></p><input type="file" id="fileElem" multiple accept="image/*" onchange="handleFiles(this.files)">';
        dropArea.classList.add('highlight');
        upload = true;
    }
}

function dragleave() {
    if (upload) {
        dropArea.classList.remove('highlight');
        dropArea.innerHTML = dropAreaText;
        upload = false;
    }
}

function handleDrop(e) {
    if (upload) {
        dropArea.classList.remove('highlight');
        dropArea.innerHTML = dropAreaText;

        //handle files
        let dt = e.dataTransfer;
        let files = dt.files;
        handleFiles(files);
        upload = false;
    }
}

function handleFiles(files) {
    ([...files]).forEach(uploadFile);
}

function uploadFile(file) {
    let url = 'api/FlightPlan';
    let xhr = new XMLHttpRequest();
    let formData = new FormData;
    xhr.open('POST', url);
    xhr.setRequestHeader("Content-Type", "application/json");
    xhr.addEventListener('readystatechange',
        function (e) {
            if (xhr.readyState == 4 && (xhr.status == 200 || xhr.status == 201 || xhr.status == 202)) {
                console.log(e.srcElement.response);
                getFlights();
            } else if (xhr.readyState == 4 && (xhr.status != 200 && xhr.status != 201 && xhr.status != 202)) {
                alert(e.srcElement.response);
            }
        });
    formData.append('file', file);
    xhr.send(file);
}