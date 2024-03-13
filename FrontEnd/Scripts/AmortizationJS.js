document.getElementById('calculate-button').addEventListener('click', async function() {
    var loan = {
        loanAmount: document.getElementById('loanAmount-input').value,
        startDate: document.getElementById('startDate-input').value,
        endDate: document.getElementById('endDate-input').value,
        interestRate: document.getElementById('interestRate-input').value,
        downPayment: document.getElementById('downPayment-input').value,
        paymentsPerYear: document.getElementById('paymentsPerYear-input').value
    };

    const response = await fetch('https://localhost:7172/api/Calculation/add-amortization-plan', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(loan),
    });

    if (response.ok) {
        let data;
        const contentType = response.headers.get("content-type");
        if (contentType && contentType.indexOf("application/json") !== -1) {
            data = await response.json();
        } else {
            data = await response.text();
        }
        console.log('Success:', data);
        window.location.href = '../Pages/Results.html';
    } else {
        console.error('Error:', response.status, response.statusText);
    }
});
