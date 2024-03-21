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

  constructor(private showDataService: ShowDataService) { }

  ngOnInit(): void {
    this.showDataService.getData().subscribe((data) => {
      this.data = data;
    });
  }

  changePayment(item: any) {
    this.showDataService.changePayment(item.id, item.newLoanMonth, item.newPaymentAmount).subscribe(
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
