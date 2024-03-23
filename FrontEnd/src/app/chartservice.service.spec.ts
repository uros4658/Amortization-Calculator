import { TestBed } from '@angular/core/testing';

import { ChartserviceService } from './chartservice.service';

describe('ChartserviceService', () => {
  let service: ChartserviceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ChartserviceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
