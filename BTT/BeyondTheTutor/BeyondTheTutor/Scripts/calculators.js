﻿function getWeightedGrade() {
    jQuery.ajaxSettings.traditional = true

    gradesArray = [];
    weightsArray = [];

    selectElement = document.querySelectorAll(".grades");
    for (i = 0; i < 10; i++) {

        selection = selectElement[i].value;
        gradesArray[i] = selection;
    }

    selectElement = document.querySelectorAll(".weights");
    for (i = 0; i < 10; i++) {

        selection = selectElement[i].value;
        weightsArray[i] = selection;
    }

    $.ajax({
        type: "GET",
        dataType: "json",
        url: "/Calculators/WeightedGradeResults",
        data: { 'gradesArray': gradesArray, 'weightsArray': weightsArray },
        success: showWeightedGradeResults,
        error: errorOnAjax
    });
};

function getFinalGrade() {
    selectCurrentGrade = document.querySelector('#currentGrade');
    selectGoalGrade = document.querySelector('#goalGrade');
    selectFinalWeight = document.querySelector('#finalWeight');

    currentGrade = selectCurrentGrade.value;
    goalGrade = selectGoalGrade.value;
    finalWeight = selectFinalWeight.value;

    $.ajax({
        type: "GET",
        dataType: "json",
        url: "/Calculators/FinalGradeResults",
        data: { 'currentGrade': currentGrade, 'goalGrade': goalGrade, 'finalWeight': finalWeight },
        success: showFinalGradeResults,
        error: errorOnAjax
    });
};

function getCurrentGPA() {
    jQuery.ajaxSettings.traditional = true

    currentGradesArray = [];
    currentCreditsArray = [];

    selectElement = document.querySelectorAll(".currentSemesterGrades");

    for (i = 0; i < 6; i++) {

        selection = selectElement[i].value;
        currentGradesArray[i] = selection;
    }

    selectElement = document.querySelectorAll(".currentSemesterCredits");

    for (i = 0; i < 6; i++) {

        selection = selectElement[i].value;
        currentCreditsArray[i] = selection;
    }

    $.ajax({
        type: "GET",
        dataType: "json",
        url: "/Calculators/CurrentGPAResults",
        data: { 'currentGradesArray': currentGradesArray, 'currentCreditsArray': currentCreditsArray },
        success: showCurrentGPAResults,
        error: errorOnAjax
    });
};

function getCumulativeGPA() {
    jQuery.ajaxSettings.traditional = true

    selectCurrentGPA = document.querySelector('#calculated');
    selectPreviousGPA = document.querySelector('#previousGPA');
    selectPreviousCredits = document.querySelector('#previousCredits');

    currentGPA = selectCurrentGPA.value;
    previousGPA = selectPreviousGPA.value;
    previousCredits = selectPreviousCredits.value;

    currentCreditsArray = [];
    selectElement = document.querySelectorAll(".currentSemesterCredits");

    for (i = 0; i < 6; i++) {

        selection = selectElement[i].value;
        currentCreditsArray[i] = selection;
    }

    $.ajax({
        type: "GET",
        dataType: "json",
        url: "/Calculators/CumulativeGPAResults",
        data: { 'currentGPA': currentGPA, 'previousGPA': previousGPA, 'previousCredits': previousCredits, 'calculated': currentGPA, 'currentCreditsArray': currentCreditsArray },
        success: showCumulativeGPAResults,
        error: errorOnAjax
    });
};

function saveWeightedGrade() {
    selectGrade = document.querySelector('#calculatedWeightedGrade');
    selectClass = document.querySelector('#classForWeightedGrade');

    currentGrade = selectGrade.value;
    currentClass = selectClass.value;

    $.ajax({
        type: "GET",
        dataType: "json",
        url: "/WeightedGrades/SaveWeightedGrade",
        data: { 'currentGrade': currentGrade, 'currentClass': currentClass },
        success: showSuccessfulSavedGrade,
        error: errorOnAjax
    });
};

function saveFinalGrade() {
    selectGrade = document.querySelector('#calculatedFinalGrade');
    selectClass = document.querySelector('#classForFinalGrade');

    currentGrade = selectGrade.value;
    currentClass = selectClass.value;

    $.ajax({
        type: "GET",
        dataType: "json",
        url: "/FinalGrades/SaveFinalGrade",
        data: { 'currentGrade': currentGrade, 'currentClass': currentClass },
        success: showSuccessfulFinalGrade,
        error: errorOnAjax
    });
};

function saveFinalGPA() {
    selectCumulativeGPA = document.querySelector('#calculatedFinalGPA');

    cumulativeGPA = selectCumulativeGPA.value;

    $.ajax({
        type: "GET",
        dataType: "json",
        url: "/CumulativeGPAs/SaveCumulativeGPA",
        data: { 'cumulativeGPA': cumulativeGPA },
        success: showSuccessfulFinalGPA,
        error: errorOnAjax
    });
};

function errorOnAjax() {
    console.log("ERROR in ajax request.");
}

function showWeightedGradeResults(data) {
    document.getElementById("weighted").remove();
    $('#weightedTable').append($('<table id=\"weighted\">'));
    $('#weighted').append($('<tr id=\"tableTr\">'));
    $('#weighted').append($('</tr>'));
    $('#weightedTable').append($('</table>'));

    var table = document.getElementById("weighted");
    var row = table.insertRow(1);
    var cell = row.insertCell(0);
    cell.innerHTML = '<center>' + data;
}

function showFinalGradeResults(data) {
    document.getElementById("final").remove();
    $('#finalTable').append($('<table id=\"final\">'));
    $('#final').append($('<tr id=\"tableTr\">'));
    $('#final').append($('</tr>'));
    $('#finalTable').append($('</table>'));

    var table = document.getElementById("final");
    var row = table.insertRow(1);
    var cell = row.insertCell(0);
    cell.innerHTML = '<center>' + data;
}

function showCurrentGPAResults(data) {
    document.getElementById("currentGPA").remove();
    $('#currentGPATable').append($('<table id=\"currentGPA\">'));
    $('#currentGPA').append($('<tr id=\"tableTr\">'));
    $('#currentGPA').append($('</tr>'));
    $('#currentGPATable').append($('</table>'));

    var table = document.getElementById("currentGPA");
    var row = table.insertRow(1);
    var cell = row.insertCell(0);
    cell.innerHTML = '<center>' + data;
}

function showCumulativeGPAResults(data) {
    document.getElementById("cumulative").remove();
    $('#cumulativeTable').append($('<table id=\"cumulative\">'));
    $('#cumulative').append($('<tr id=\"tableTr\">'));
    $('#cumulative').append($('</tr>'));
    $('#cumulativeTable').append($('</table>'));

    var table = document.getElementById("cumulative");
    var row = table.insertRow(1);
    var cell = row.insertCell(0);
    cell.innerHTML = '<center>' + data;
}

function showSuccessfulSavedGrade(data) {
    document.getElementById("savedGrade").remove();
    $('#savedGradeTable').append($('<table id=\"savedGrade\">'));
    $('#savedGrade').append($('<tr id=\"tableTr\">'));
    $('#savedGrade').append($('</tr>'));
    $('#savedGradeTable').append($('</table>'));

    var table = document.getElementById("savedGrade");
    var row = table.insertRow(1);
    var cell = row.insertCell(0);
    if (data == "must enter values to view results") {
        cell.innerHTML = '<center>' + data;
    }
    else {
        cell.innerHTML = '<center>' + data + '<a href="/Calculators/SavedResults" target="_blank">Click here </a>' + 'to view saved results';
    }  
}

function showSuccessfulFinalGrade(data) {
    document.getElementById("savedFinal").remove();
    $('#savedFinalTable').append($('<table id=\"savedFinal\">'));
    $('#savedFinal').append($('<tr id=\"tableTr\">'));
    $('#savedFinal').append($('</tr>'));
    $('#savedFinalTable').append($('</table>'));

    var table = document.getElementById("savedFinal");
    var row = table.insertRow(1);
    var cell = row.insertCell(0);
    if (data == "must enter values to view results") {
        cell.innerHTML = '<center>' + data;
    }
    else {
        cell.innerHTML = '<center>' + data + '<a href="/Calculators/SavedResults" target="_blank">Click here </a>' + 'to view saved results';
    }
}

function showSuccessfulFinalGPA(data) {
    document.getElementById("savedFinalGPA").remove();
    $('#savedFinalGPATable').append($('<table id=\"savedFinalGPA\">'));
    $('#savedFinalGPA').append($('<tr id=\"tableTr\">'));
    $('#savedFinalGPA').append($('</tr>'));
    $('#savedFinalGPATable').append($('</table>'));

    var table = document.getElementById("savedFinalGPA");
    var row = table.insertRow(1);
    var cell = row.insertCell(0);
    if (data == "must enter value to view results") {
        cell.innerHTML = '<center>' + data;
    }
    else {
        cell.innerHTML = '<center>' + data + '<a href="/Calculators/SavedResults" target="_blank">Click here </a>' + 'to view saved results';
    }
}

$(function () {
    $('#resetWeighted').on('click', function () {
        $('input[type="number"]').val('');
    });
});

$(function () {
    $('#resetFinal').on('click', function () {
        $('input[id="currentGrade"]').val('');
        $('input[id="goalGrade"]').val('');
        $('input[id="finalWeight"]').val('');
    });
});

$(function () {
    $('#resetCurrentGPA').on('click', function () {
        $('input[class="currentSemesterGrades"]').val('');
        $('input[class="currentSemesterCredits"]').val('');
    });
});

$(function () {
    $('#resetCumulativeGPA').on('click', function () {
        $('input[id="calculated"]').val('');
        $('input[id="previousGPA"]').val('');
        $('input[id="previousCredits"]').val('');
    });
});
