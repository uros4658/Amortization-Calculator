// loan-form.component.ts
import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Loan } from '../_models/loan';
import { LoanService } from '../_services/loan.service';
import { Router, ActivatedRoute } from '@angular/router'; // import Router and ActivatedRoute

@Component({
  selector: 'app-loan-form',
  templateUrl: './loan-form.component.html',
  styleUrls: ['./loan-form.component.css']
})
export class LoanFormComponent implements OnInit {
  loan: Loan = new Loan();
  loans: Loan[] = [];
  showLoans = false;

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
  

  onSubmit(form: NgForm) {
    if (form.valid) {
      this.loan.username = localStorage.getItem('username')!; // get username from local storage
      this.loanService.addAmortizationPlan(this.loan).subscribe(
        response => {
          console.log(response);
          this.loanService.getLastLoanID().subscribe(
            lastLoanID => {
              localStorage.setItem('loanID', lastLoanID.toString());
              this.router.navigate(['../showdata'], { relativeTo: this.route });
            },
            error => {
              console.error(error);
            }
          );
        },
        error => {
          console.error(error);
        }
      );
    }
  }
  
}
