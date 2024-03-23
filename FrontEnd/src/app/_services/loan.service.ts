// loan.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Loan } from '../_models/loan';

@Injectable({
  providedIn: 'root'
})
export class LoanService {
  private apiUrl = 'https://localhost:7172/api/Calculation';

  constructor(private http: HttpClient) { }

  addAmortizationPlan(loan: Loan) {
    return this.http.post(`${this.apiUrl}/add-amortization-plan`, loan);
  }

  getUserLoans(username: string) {
    return this.http.get<Loan[]>(`${this.apiUrl}/${username}/loans`);
  }
  getLastLoanID() {
    return this.http.get('https://localhost:7172/api/Calculation/last-loan-id', {});
  }
}
