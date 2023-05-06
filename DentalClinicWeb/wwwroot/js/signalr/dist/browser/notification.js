// Define the connection and hub
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationsHub")
    .build();

// Define the function to handle the notification message received from the server
connection.on("ReceiveNotification", (message) => {
    // Update the bell-fill icon with the notification count
    const badgeElement = document.querySelector(".badge-custom");
    let count = parseInt(badgeElement.innerText);
    count++;
    badgeElement.innerText = count;

    // Show the notification message
    const dropdownMenu = document.querySelector("#notificationsDropdown .dropdown-menu");
    dropdownMenu.insertAdjacentHTML("beforeend", `<a class="dropdown-item" href="#">${message}</a>`);
});
