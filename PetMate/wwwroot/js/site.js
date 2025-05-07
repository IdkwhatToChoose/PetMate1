// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function SeeCheckedAnswers() {
    let sell = [];
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




function updateAnsweredCount() {
    const totalQuestions = 15; // Total number of questions
    let answeredQuestions = 0;

    // Select all question containers
    const questions = document.querySelectorAll('.question');

    questions.forEach(question => {
        // Check if any input inside the question is checked
        const isAnswered = question.querySelector('input[type="radio"]:checked, input[type="checkbox"]:checked');
        if (isAnswered) {
            answeredQuestions++;
        }
    });

    // Update the count in the <strong> element
    const qAnsweredElement = document.getElementById('qAnswered');
    qAnsweredElement.textContent = `${answeredQuestions}/${totalQuestions}`;
    if (answeredQuestions === totalQuestions) {
        qAnsweredElement.style.color = 'green';
        document.querySelector('#vField').value = "true";
        document.querySelector('#submit').disabled = false; // Enable the submit button if all questions are answered
    } else {
        qAnsweredElement.style.color = 'red';
        document.querySelector('#vField').value = "false";
        document.querySelector('#submit').disabled = true; // Disable the submit button if not all questions are answered
    }
}

// Add event listeners to all inputs to track changes
document.querySelectorAll('input[type="radio"], input[type="checkbox"]').forEach(input => {
    input.addEventListener('change', updateAnsweredCount);
});

// Initialize the count on page load
document.addEventListener('DOMContentLoaded', updateAnsweredCount);



//card.addEventListener('change', function (event) {
//    var displayError = document.getElementById('card-errors');
//    if (event.error) {
//        displayError.textContent = event.error.message;
//    } else {
//        displayError.textContent = '';
//    }
//});

//var form = document.getElementById('donation-form');
//form.addEventListener('submit', function (event) {
//    event.preventDefault();

//    stripe.createToken(card).then(function (result) {
//        if (result.error) {
//            var errorElement = document.getElementById('card-errors');
//            errorElement.textContent = result.error.message;
//        } else {
//            stripeTokenHandler(result.token);
//        }
//    });
//});

//function stripeTokenHandler(token) {
//    var form = document.getElementById('donation-form');
//    var hiddenInput = document.getElementById("token");
//    hiddenInput.value = token.id;

//    form.submit();
//}
////-------------------------------------------------------------------------

//const elements = stripe.elements();
//const cardElement = elements.create('card');
//cardElement.mount('#card-element');

//// Handle amount option clicks
//const amountOptions = document.querySelectorAll('.amount-option');
//const amountInput = document.getElementById('amount');

//amountOptions.forEach(option => {
//    option.addEventListener('click', function () {
//        // Remove active class from all options
//        amountOptions.forEach(opt => opt.classList.remove('active'));

//        // Add active class to clicked option
//        this.classList.add('active');

//        // Update amount input
//        amountInput.value = this.dataset.amount;
//    });
//});

//// Handle form submission
//const form = document.getElementById('donation-form');
//const submitButton = document.getElementById('submit-button');
//const buttonText = document.querySelector('.button-text');
//const buttonLoading = document.querySelector('.button-loading');
//const cardErrors = document.getElementById('card-errors');

//form.addEventListener('submit', async function (e) {
//    e.preventDefault();

//    stripe.createToken(cardElement).then(function (result) {
//        if (result.error) {
//            var errorElement = document.getElementById('card-errors');
//            errorElement.textContent = result.error.message;
//        } else {
//            stripeTokenHandler(result.token);
//        }
//    });
//});

//// Set default amount option to active
//document.querySelector('.amount-option[data-amount="100"]').click();