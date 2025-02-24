
/**
 * @param {HTMLButtonElement} button
 */

// let clickedButtonIds = new Set(); // Store multiple clicked button IDs

// function setClicked(button) {
//     const pairContainer = button.closest('.Btn__container');
//     const buttonsInPair = pairContainer.querySelectorAll('button');

//     const buttonId = button.getAttribute("data-id");

//     // Toggle selection in the set
//     if (clickedButtonIds.has(buttonId)) {
//         clickedButtonIds.delete(buttonId); // Remove if already clicked
//     } else {
//         clickedButtonIds.add(buttonId); // Add to selected buttons
//     }

//     console.log("Selected button IDs:", Array.from(clickedButtonIds));

//     // Toggle "clicked" class
//     if (button.classList.contains('clicked')) {
//         button.classList.remove('clicked');
//     } else {
//         buttonsInPair.forEach(btn => btn.classList.remove('clicked'));
//         button.classList.add('clicked');
//     }
// }

// function setClickedBet(button) {
//     button.classList.toggle("clicked");

//     // Log the current selected buttons (IDs)
//     console.log("SetClickedBet: Last clicked button IDs:", Array.from(clickedButtonIds));

//     // Prepare the data for the POST request
//     const data = {
//         selectedButtonIds: Array.from(clickedButtonIds) // Convert Set to Array
//     };

//     // 0 П1 1 П2 2 П1 3 П2

//     fetch('/Home/AddToDb', {
//         method: 'POST',
//         headers: {
//             'Content-Type': 'application/json'
//         },
//         body: JSON.stringify(data) // Send data as JSON string
//     })
//         .then(response => {
//             if (!response.ok) {
//                 throw new Error('Network response was not ok');
//             }
//             // Optional: Do something after the request completes
//             console.log("Data processed and redirecting...");
//             window.location.href = '/Home/Index'; // Redirects to the Index page
//         })
//         .catch(error => {
//             console.error("Error during POST request:", error);
//         });
// }


let clickedButtons = new Map(); // Store clicked button IDs and their text

function setClicked(button) {
    const pairContainer = button.closest('.Btn__container');
    const buttonsInPair = pairContainer.querySelectorAll('button');

    const buttonId = button.getAttribute("data-id");
    const buttonText = button.innerText.trim(); // Get button text (trim to remove spaces)

    // Toggle selection in the dictionary
    if (clickedButtons.has(buttonId)) {
        clickedButtons.delete(buttonId); // Remove if already clicked
    } else {
        clickedButtons.set(buttonId, buttonText); // Add ID and Text
    }

    console.log("Selected button data:", Object.fromEntries(clickedButtons));

    // Toggle "clicked" class
    if (button.classList.contains('clicked')) {
        button.classList.remove('clicked');
    } else {
        buttonsInPair.forEach(btn => btn.classList.remove('clicked'));
        button.classList.add('clicked');
    }
}

function setClickedBet(button) {
    button.classList.toggle("clicked");

    // Convert Map to an array of objects for better readability
    const SelectedButtons = Object.fromEntries(clickedButtons);

    console.log("SetClickedBet: Selected button data:", SelectedButtons);

    // Prepare the data for the POST request
    const data = {
        selectedButtons: SelectedButtons // Send as a dictionary
    };

    fetch('/Bets/AddToDb', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ SelectedButtonIds: SelectedButtons }) 
    })

        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            console.log("Data processed and redirecting...");
        })
        .catch(error => {
            console.error("Ты еблае вот ошибка", error);
        });
}
