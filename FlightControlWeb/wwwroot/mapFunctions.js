﻿let markedMarker = null;
let markedFlight = "";
let isMarked = false;
let markers = [];
let map;
let flightPath = null;
let destMarker = null;

//icons
let planeIcon;
let markedIcon;
let endIcon;
let pointIcon;


//sets the map on all the markers in the array
function setMapOnAll(map) {
    for (marker of markers) {
        marker.setMap(map);
    }
}

// deletes all currnt markers
function deleteMarkers() {
    setMapOnAll(null);
    markers = [];
}

//creates a marker
function addMarker(flight) {
    let marker = new google.maps.Marker({
        position: { lat: flight.latitude, lng: flight.longitude },
        map: map,
        icon: planeIcon
    });

    marker.addListener('click', function () {
        markFlight(this, flight.flight_id);
    });

    markers.push(marker);
    return marker;
}

// creates the plane route on the map
function makePath(segments, initial_location, flight_id) {
    let len = segments.length;
    let flightPlanCoordinates = [];
    let segList = "";

    flightPlanCoordinates.push({ lat: initial_location.latitude, lng: initial_location.longitude});
    for (let segment of segments) {
        flightPlanCoordinates.push({ lat: segment.latitude, lng: segment.longitude });
        segList += '<li class="list-group-item">';
        segList += '<p class="p" style="font-size:2vh;margin-bottom:0;">';
        segList += '<b>Location:</b> ' + segment.latitude + ' / ' + segment.longitude;
        segList += '<br/>';
        segList += '<b>Time span: </b>' + segment.timespan_seconds;
    }
    document.getElementById("segments").innerHTML = segList;
    let dest = flightPlanCoordinates[segments.length];
    // setting destination markers position
    destMarker.setPosition(dest);
    if (flight_id !== markedFlight) { document.getElementById("segments").scrollTo(top);}
    flightPath.setPath(flightPlanCoordinates);
}


// sets the marked flight info in the bottom table
function setFlightPlan(flightPlan, flight_id) {
    if (flightPlan !== null) {
        document.getElementById("flight_id").innerHTML = flight_id;
        document.getElementById("passengers").innerHTML = flightPlan.passengers;
        document.getElementById("company_name").innerHTML = flightPlan.company_name;
        let location = flightPlan.initial_location.longitude + ' / ';
        location += flightPlan.initial_location.latitude;
        document.getElementById("initial_location").innerHTML = location;
        document.getElementById("data_time").innerHTML = flightPlan.initial_location.date_time;
    } else {
        document.getElementById("flight_id").innerHTML = "";
        document.getElementById("passengers").innerHTML = "";
        document.getElementById("company_name").innerHTML = "";
        document.getElementById("initial_location").innerHTML = "";
        document.getElementById("data_time").innerHTML = "";
    }
}


// remove the mark from the marked plane
function removeMark() {
    if (isMarked) {
        // dealing with map marker
        //markedMarker.setAnimation(null);
        markedMarker.setIcon(planeIcon);

        //dealing with flight route on the map
        flightPath.setMap(null);
        destMarker.setMap(null);

        // sets the flight plan on the bottom
        document.getElementById("segments").innerHTML = "";
        setFlightPlan(null);

        // dealing with flights list
        document.getElementById(markedFlight).setAttribute("class", "list-group-item");

        isMarked = false;
    }
}

// marks the given plane in the map and flights
function mark(marker, flight_id, flightplan) {
    // dealing with map marker
    //marker.setAnimation(google.maps.Animation.BOUNCE);
    marker.setIcon(markedIcon);
    markedMarker = marker;

    //dealing with flight route on the map
    flightPath = new google.maps.Polyline({
        //geodesic: true,
        strokeColor: '#FF0000',
        strokeOpacity: 1,
        strokeWeight: 2
    });
    destMarker = new google.maps.Marker({
        icon: endIcon
    });
    makePath(flightplan.segments, flightplan.initial_location, flight_id);
    flightPath.setMap(map);
    destMarker.setMap(map);

    // sets the flight plan on the bottom
    setFlightPlan(flightplan, flight_id);

    // dealing with flights list
    markedFlight = flight_id;
    document.getElementById(markedFlight).setAttribute("class", "list-group-item active");
    
    isMarked = true;
}

// switches marks from the currnt marked plane to the pressed one
function switchMark(marker, flight_id, flightplan) {
    removeMark();
    mark(marker, flight_id, flightplan);
}


//function - when a marker is pressed, the function makes it bounce and colors it
function markFlight(marker, flight_id) {
    let xhttp = new XMLHttpRequest();
    xhttp.onreadystatechange = function () {
        if (this.readyState === 4 && (this.status === 200 || this.status === 201
            || this.status === 202)) {
            let flightplan = JSON.parse(this.responseText);
            switchMark(marker, flight_id, flightplan);
        } else if (this.readyState === 4 && this.status === 404) {
            showSnackBar("ERROR - Could not get flight plan from the server, trying again...", 3);
        } else if (this.readyState === 4 && (this.status !== 200 && this.status !== 201
            && this.status !== 202)) {
            showSnackbar("Something went wrong, trying again...", 3);
            console.log(this.responseText);
        }
    };
    xhttp.open("GET", "/api/FlightPlan/" + flight_id, true);
    xhttp.send();
}

function defineIcons() {
    //icons
    planeIcon = {
        url: '/images/planeicon.png', // url
        scaledSize: new google.maps.Size(20, 20), // scaled size
        origin: new google.maps.Point(0, 0), // origin
        anchor: new google.maps.Point(10, 10) // anchor
    };
    markedIcon = {
        url: '/images/markedplane.png',// url
        scaledSize: new google.maps.Size(20, 20), // scaled size
        origin: new google.maps.Point(0, 0), // origin
        anchor: new google.maps.Point(10, 10) // anchor
    };
    endIcon = {
        url: '/images/destinationicon.png',
        scaledSize: new google.maps.Size(30, 30),
        origin: new google.maps.Point(0, 0),
        anchor: new google.maps.Point(4, 25)
    };
}

//instantiatint map
function initMap() {

    //map settings
    let options = {
        zoom: 8,
        center: { lat: 31.4117, lng: 35.0818 },
        styles: [
            {
                elementType: 'geometry',
                stylers: [{ color: '#f5f5f5' }]
            },
            {
                elementType: 'labels.icon',
                stylers: [{ visibility: 'off' }]
            },
            {
                elementType: 'labels.text.fill',
                stylers: [{ color: '#616161' }]
            },
            {
                elementType: 'labels.text.stroke',
                stylers: [{ color: '#f5f5f5' }]
            },
            {
                featureType: 'administrative.land_parcel',
                elementType: 'labels.text.fill',
                stylers: [{ color: '#bdbdbd' }]
            },
            {
                featureType: 'poi',
                elementType: 'geometry',
                stylers: [{ color: '#eeeeee' }]
            },
            {
                featureType: 'poi',
                elementType: 'labels.text.fill',
                stylers: [{ color: '#757575' }]
            },
            {
                featureType: 'poi.park',
                elementType: 'geometry',
                stylers: [{ color: '#e5e5e5' }]
            },
            {
                featureType: 'poi.park',
                elementType: 'labels.text.fill',
                stylers: [{ color: '#9e9e9e' }]
            },
            {
                featureType: 'road',
                elementType: 'geometry',
                stylers: [{ color: '#ffffff' }]
            },
            {
                featureType: 'road.arterial',
                elementType: 'labels.text.fill',
                stylers: [{ color: '#757575' }]
            },
            {
                featureType: 'road.highway',
                elementType: 'geometry',
                stylers: [{ color: '#dadada' }]
            },
            {
                featureType: 'road.highway',
                elementType: 'labels.text.fill',
                stylers: [{ color: '#616161' }]
            },
            {
                featureType: 'road.local',
                elementType: 'labels.text.fill',
                stylers: [{ color: '#9e9e9e' }]
            },
            {
                featureType: 'transit.line',
                elementType: 'geometry',
                stylers: [{ color: '#e5e5e5' }]
            },
            {
                featureType: 'transit.station',
                elementType: 'geometry',
                stylers: [{ color: '#eeeeee' }]
            },
            {
                featureType: 'water',
                elementType: 'geometry',
                stylers: [{ color: '#c9c9c9' }]
            },
            {
                featureType: 'water',
                elementType: 'labels.text.fill',
                stylers: [{ color: '#9e9e9e' }]
            }
        ]
    };

    //create map
    map = new google.maps.Map(document.getElementById('map'), options);
    map.addListener('click', function () {
        removeMark();
        markedFlight = "";
    });

    defineIcons();
}