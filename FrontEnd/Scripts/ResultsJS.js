function updateTable() {
    fetch('/api/loans')
        .then(response => response.json())
        .then(loans => {
            const table = document.querySelector('table');
            // Clear the table
            table.innerHTML = `
                <tr>
                    <th>Id</th>
                    <th>Loan Month</th>
                    <th>Monthly Payment</th>
                    <th>Amount Left</th>
                    <th>Principal</th>
                    <th>Interest</th>
                </tr>
            `;
            // Add new rows
            for (const loan of loans) {
                const row = document.createElement('tr');
                row.innerHTML = `
                    <td>${loan.Id}</td>
                    <td>${loan.LoanMonth}</td>
                    <td>${loan.MonthlyPayment}</td>
                    <td>${loan.AmountLeft}</td>
                    <td>${loan.Principal}</td>
                    <td>${loan.Interest}</td>
                `;
                table.appendChild(row);
            }
        });
}

// Call updateTable every 5 seconds
setInterval(updateTable, 5000);
