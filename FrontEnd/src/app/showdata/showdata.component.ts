import { ShowDataService } from './../showdata.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-showdata',
  templateUrl: './showdata.component.html',
  styleUrls: ['./showdata.component.css']
})
export class ShowDataComponent implements OnInit {
  data: any;

  constructor(private showDataService: ShowDataService) { }

  ngOnInit(): void {
    this.showDataService.getData().subscribe((data) => {
      this.data = data;
    });
  }
}
