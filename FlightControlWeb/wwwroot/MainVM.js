let interval;
let flights = [];

function showSnackbar(messege, sec) {
    let x = document.getElementById("snackbar");
    x.className = "show";
    x.childNodes[0].innerHTML = messege;
    // After given numbber of seconds, remove the show class from DIV
    setTimeout(function () { x.className = x.className.replace("show", ""); }, sec*1000);
}

// get the current UTC time in the required format
function getDTString(date = new Date()) {
    let date_time = "";
    date_time += date.getUTCFullYear() + "-";
    date_time += (((date.getUTCMonth() + 1) < 10) ? "0" : "") + (date.getUTCMonth() + 1) + "-";
    date_time += ((date.getUTCDate() < 10) ? "0" : "") + date.getUTCDate() + "T";
    date_time += ((date.getUTCHours() < 10) ? "0" : "") + date.getUTCHours() + ":";
    date_time += ((date.getUTCMinutes() < 10) ? "0" : "") + date.getUTCMinutes() + ":";
    date_time += ((date.getUTCSeconds() < 10) ? "0" : "") + date.getUTCSeconds() + "Z";
    return date_time;
}

// adds the HTML text of a flight
function addFlight(flight) {
    let flightHTML = "";
    let color = flight.is_external ? 'red' : 'blue';
    flightHTML += '<li id="' + flight.flight_id + '" class="list-group-item">';
    flightHTML += '<h6 class="h6" style="float:center;vertical-align: middle;">';
    flightHTML += '<i class="fa fa-plane" style="color:' + color;
    flightHTML += ';vertical-align: middle;"></i> Flight ID: ' + flight.flight_id;
    flightHTML += ', Company: ' + flight.company_name;
    return flightHTML;
}

// delete a given flight and refreshes the flights page
function deleteFlight(flight_id) {
    let xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && (this.status == 200 || this.status == 201
            || this.status == 202)) {
            // if deleted flight is currently marked
            if (flight_id == markedFlight) {
                removeMark();
                flight_id = "";
                markedFlight = "";
            }
            getFlights();
        } else if (this.readyState == 4 && this.status == 404) {
            showSnackbar("ERROR - Could not delete flight, please try again", 3);
        } else if (this.readyState == 4 && (this.status != 200 && this.status != 201
            && this.status != 202)) {
            showSnackbar("Something went wrong, please try again", 3);
            console.log(this.responseText);
        }
    };
    xhttp.open("DELETE", "/api/Flights/" + flight_id, true);
    xhttp.send();
}

function addDelete() {
    for (del of document.getElementsByClassName("delete")) {
        del.addEventListener('click', function () {
            deleteFlight(this.parentNode.parentNode.id);
            event.stopPropagation();
        });
    }
}

function addMarkers() {
    /*adding click listeners and markers to every flight and if a flight 
     * was already marked remarking it*/
    let wasMarked = false;
    for (flight of flights) {
        let marker = addMarker(flight);
        document.getElementById(flight.flight_id).addEventListener('click', function () {
            if (markedFlight != this.id) { markFlight(marker, this.id); }
        });
        if (flight.flight_id == markedFlight) {
            wasMarked = true;
            markFlight(marker, flight.flight_id);
        }
    }
    // if none of the new flights was marked removing the current mark
    if (!wasMarked) { removeMark(); }
}

// display all flights given from the server
function displayFlights() {
    let myFlights = '<ul class="list-group">';
    let externalFlights = '<ul class="list-group">';
    // iterating every flight
    for (flight of flights) {
        if (flight.is_external) {
            externalFlights += addFlight(flight);
            externalFlights += '</h6></li>';
        } else {
            myFlights += addFlight(flight);
            myFlights += '<span class="delete" style="float:right;">';
            myFlights += '<i class="fa fa-trash"></i></span ></h6 ></li > ';
        }
    }
    myFlights += '</ul>';
    externalFlights += '</ul>';
    // sets the html of my flights
    document.getElementById("myflights").innerHTML = myFlights;
    document.getElementById("externalflights").innerHTML = externalFlights;

    // add markers and mark listeners
    addMarkers();
    // add delete listeners
    addDelete();
}

// gets the relevent flights from the server
function getFlights() {
    let xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState == 4 && (this.status == 200 || this.status == 201
            || this.status == 202)) {
            deleteMarkers();
            flights = JSON.parse(this.responseText);
            displayFlights();
        } else if (this.readyState == 4 && (this.status != 200 && this.status != 201
            && this.status != 202)) {
            showSnackbar("ERROR - Could not get flights from the server. trying again...", 3);
            console.log(this.responseText);
        }
    };
    xhttp.open("GET", "/api/Flights?relative_to=" + getDTString() + "&sync_all", true);
    xhttp.send();
}