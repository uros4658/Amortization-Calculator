const usernameInput = document.getElementById('username-input');
const passwordInput = document.getElementById('password-input');
const loginButton = document.getElementById('login-button');

loginButton.addEventListener('click', async () => {
    const username = usernameInput.value;
    const password = passwordInput.value;

    // Create the request body
    const requestBody = {
        username: username,
        password: password
    };

    // Send the POST request to the login API
    const response = await fetch('https://localhost:7172/api/Authorization/login', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(requestBody)
    });

    // Check if the request was successful
    if (response.ok) {
        const data = await response.json();
        console.log('Token:', data);
        window.location.href = '../Pages/AmortizatioPage.html';
    } else {
        console.error('Error:', response.status, response.statusText);
    }
});
