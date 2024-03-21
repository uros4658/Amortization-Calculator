import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Loan } from '../_models/loan';

@Injectable({
  providedIn: 'root'
})
export class LoanService {
  private apiUrl = 'https://localhost:7172/api/Calculation/add-amortization-plan';

  constructor(private http: HttpClient) { }

  addAmortizationPlan(loan: Loan) {
    return this.http.post(this.apiUrl, loan);
  }
}
