import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { ChartService } from '../chartservice.service'; // Import your ChartService
import { Chart } from 'chart.js'; // Import Chart from chart.js
import { Payment } from '@app/_models/payment';
import 'chart.js/auto';

@Component({
  selector: 'app-chart-page',
  templateUrl: './chart-page.component.html',
  styleUrls: ['./chart-page.component.less']
})
export class ChartPageComponent implements OnInit, AfterViewInit {
  @ViewChild('chartCanvas') chartCanvas!: ElementRef; // Add '!' here
  chartData!: Payment[]; // Add '!' here
  chart: any;

  constructor(private chartService: ChartService) { }

  ngOnInit(): void {
    this.chartService.getData().subscribe(data => {
      this.chartData = data;
      if (this.chartData && this.chartData.length > 0) {
        this.createChart(); // Call createChart after you get the data
      }
    });
  }

  ngAfterViewInit(): void {
    this.createChart(); // Call createChart after you get the data
  }

  createChart() {
    const labels = this.chartData.map((payment: Payment) => {
      const date = new Date(payment.loanMonth);
      return `${date.getDate()}/${date.getMonth() + 1}/${date.getFullYear()}`;
    });
    const interestData = this.chartData.map((payment: Payment) => payment.interest);
    const principalData = this.chartData.map((payment: Payment) => payment.principal);
    const monthlyPaymentData = this.chartData.map((payment: Payment) => payment.monthlyPayment);

    this.chart = new Chart(this.chartCanvas.nativeElement, {
      type: 'line',
      data: {
        labels: labels,
        datasets: [
          {
            label: 'Interest',
            data: interestData,
            borderColor: '#3cba9f',
            fill: false
          },
          {
            label: 'Principal',
            data: principalData,
            borderColor: '#ffcc00',
            fill: false
          },
          {
            label: 'Monthly Payment',
            data: monthlyPaymentData,
            borderColor: '#c45850',
            fill: false
          }
        ]
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
      }
    });
  }
}
