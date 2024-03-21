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

  constructor(
    private loanService: LoanService,
    private router: Router, // inject Router
    private route: ActivatedRoute // inject ActivatedRoute
  ) { }

  ngOnInit(): void {
  }

  onSubmit(form: NgForm) {
    if (form.valid) {
      this.loanService.addAmortizationPlan(this.loan).subscribe(
        response => {
          console.log(response);
          this.router.navigate(['../showdata'], { relativeTo: this.route }); // navigate to ../showdata
        },
        error => {
          console.error(error);
        }
      );
    }
  }
}
