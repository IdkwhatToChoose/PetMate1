﻿// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function Call() {
    Validate();
    SeeCheckedAnswers();
}

function SeeCheckedAnswers() {
    let sell = [];
    const questions = document.querySelectorAll(".question span:first-child");
    const checkboxesValue = document.querySelectorAll("input[type='radio']:checked + span,input[type='checkbox']:checked + span");
    let p = document.getElementById('answers');
    let counter = 1;
    // Loop through each selected span and get its text content
    checkboxesValue.forEach(span => {
        sell.push(`${counter}. ` + span.textContent.trim()); // Add text content to the array
        counter++;
    });

    p.value = sell.join(", ");
}

function Validate() {
    let radios = document.querySelectorAll('.option');
    let text = document.querySelector('p');
    let count = 0;
    radios.forEach((radio) => {
        if (radio.checked) {
            count++;
        }
    });
    if (count >=15) {
        text.innerHTML = 'You have selected all the radio buttons';
    } else {
        text.innerHTML = 'You have not selected all the radio buttons';
    }
}