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
        let text = "";
        text += '<div class="container" style="pointer-events: none;">';
        text += '<form class="my-form"><p>Upload files by dragging them onto the dashed area</p>';
        text += '<input type="file" id="fileElem" multiple accept="image/*"';
        text += 'onchange = "handleFiles(this.files)" >'
        dropArea.innerHTML = text;
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
        function () {
            if (this.readyState == 4 && (this.status == 200 || this.status == 201
                || this.status == 202)) {
                getFlights();
            } else if (this.readyState == 4 && (this.status != 200 && this.status != 201
                && this.status != 202)) {
                showSnackbar("ERROR - Could not upload file, please try again", 3);
                console.log(this.responseText);
            }
        });
    formData.append('file', file);
    xhr.send(file);
}