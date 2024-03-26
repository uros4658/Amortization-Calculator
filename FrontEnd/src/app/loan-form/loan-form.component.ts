// loan-form.component.ts
import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Loan } from '../_models/loan';
import { LoanService } from '../_services/loan.service';
import { Router, ActivatedRoute } from '@angular/router';
import { MiscCost } from '@app/_models/misccost';

@Component({
  selector: 'app-loan-form',
  templateUrl: './loan-form.component.html',
  styleUrls: ['./loan-form.component.css']
})
export class LoanFormComponent implements OnInit {
  loan: Loan = new Loan();
  loans: Loan[] = [];
  showLoans = false;
  miscCosts: MiscCost[] = [new MiscCost()];


  constructor(
    private loanService: LoanService,
    private router: Router, // inject Router
    private route: ActivatedRoute // inject ActivatedRoute
  ) { }

  ngOnInit(): void {
    // this.getUserLoans();
  }

  getUserLoans() {
    const username = localStorage.getItem('username')!;
    this.loanService.getUserLoans(username).subscribe(
      response => {
        this.loans = response;
        localStorage.setItem('loanID', this.loans[this.loans.length - 1].id.toString());
        this.showLoans = true;
      },
      error => {
        console.error(error);
      }
    );
  }

  onRowClick(loanID: number) {
    localStorage.setItem('loanID', loanID.toString());
    this.router.navigate(['../showdata'], { relativeTo: this.route });
  }
  addMiscCost() {
    this.miscCosts.push(new MiscCost());
  }
  removeMiscCost(index: number) {
      this.miscCosts.splice(index, 1);
  }
  
  onSubmit(form: NgForm) {
    if (form.valid) {
      this.loan.username = localStorage.getItem('username')!; // get username from local storage
  
      // Step 1: Create the loan
      this.loanService.createLoan(this.loan).subscribe(
        response => {
          console.log(response);
  
          // Step 2: Create the misc costs if they exist
          for (let miscCost of this.miscCosts) {
            miscCost.loanID = this.loan.id;
            this.loanService.createMisc(miscCost).subscribe(
              response => {
                console.log(response);
              },
              error => {
                console.error(error);
              }
            );
          }
  
          // Step 3: Add the payment plan
          this.loanService.addPaymentPlan(this.loan).subscribe(
            response => {
              console.log(response);
              this.router.navigate(['../showdata'], { relativeTo: this.route });
            },
            error => {
              console.error(error);
            }
          );
        },
        error => {
          console.error(error);
          console.log("erorr");
        }
      );
    }
    localStorage.setItem('loanID', this.loan.id.toString());
  }
  moveToShowData(){
    localStorage.setItem('loanID', '1');
    this.router.navigate(['../showdata'], { relativeTo: this.route });
  }
}
