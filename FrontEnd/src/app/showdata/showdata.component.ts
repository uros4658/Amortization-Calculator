import { ShowDataService } from './../showdata.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-showdata',
  templateUrl: './showdata.component.html',
  styleUrls: ['./showdata.component.css']
})
export class ShowDataComponent implements OnInit {
  data: any;
  p: number = 1;
  effectiveInterestRate: number = 0;
  showForm: boolean = false;  
  selectedItem = {id : 0, newLoanMonth: Date, newPaymentAmount: 0};

  constructor(private showDataService: ShowDataService) { }
  

  ngOnInit(): void {
    this.showDataService.getData().subscribe((data) => {
      this.data = data;
    });
    console.log(localStorage.getItem('loanID'));
  }

  
  changePayment(selectedItem: any) {
    this.showDataService.changePayment(selectedItem.id, selectedItem.newLoanMonth, selectedItem.newPaymentAmount).subscribe(
      response => {
        console.log(response);
        // Refresh the page or update the data
        location.reload();
      },
      error => {
        console.error(error);
        // Handle error
      }
    );  
  }

  calculateAndDisplayInterestRate(): void {
    console.log(localStorage.getItem('loanID'));
    let loanID = localStorage.getItem('loanID');
    if (loanID !== null) {
        let loanIDAsNumber = parseInt(loanID);
        this.showDataService.calculateEffectiveInterestRate(loanIDAsNumber).subscribe(interestRate => {
            this.effectiveInterestRate = interestRate;
        });
    }
}
  changeOneMonthPayment(selectedItem: any) {
    this.showDataService.changeOneMonthPayment(selectedItem.id, selectedItem.newLoanMonth, selectedItem.newPaymentAmount).subscribe(
      response => {
        console.log(response);
        // Refresh the page or update the data
        location.reload();
      },
      error => {
        console.error(error);
        // Handle error
      }
    );
  }
  missMonthsPaymentSameLength(selectedItem: any) {
    this.showDataService.missMonthsPaymentSameLength(selectedItem.id, selectedItem.newLoanMonth, selectedItem.newPaymentAmount).subscribe(
      response => {
        console.log(response);
        // Refresh the page or update the data
        location.reload();
      },
      error => {
        console.error(error);
        // Handle error
      }
    );
  }
  missMonthsPaymentExtendLength(selectedItem: any) {
    this.showDataService.missMonthsPaymentExtendLength(selectedItem.id, selectedItem.newLoanMonth, selectedItem.newPaymentAmount).subscribe(
      response => {
        console.log(response);
        // Refresh the page or update the data
        location.reload();
      },
      error => {
        console.error(error);
        // Handle error
      }
    );
  }
}
