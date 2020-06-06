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
        dropArea.innerHTML = '<div class="container" style="pointer-events: none;"> <form class="my-form"><p>Upload files by dragging them onto the dashed area</p><input type="file" id="fileElem" multiple accept="image/*" onchange="handleFiles(this.files)"><label class="button" for="fileElem">Select some files</label>';
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
            if (this.readyState == 4 && (this.status == 200 || this.status == 201 || this.status == 202)) {
                getFlights();
            } else if (this.readyState == 4 && (this.status != 200 && this.status != 201 && this.status != 202)) {
                showSnackbar("ERROR - Could not upload file, please try again", 3);
                console.log(this.responseText);
            }
        });
    formData.append('file', file);
    xhr.send(file);
}