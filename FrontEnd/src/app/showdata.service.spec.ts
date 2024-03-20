import { TestBed } from '@angular/core/testing';

import { ShowdataService } from './showdata.service';

describe('ShowdataService', () => {
  let service: ShowdataService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ShowdataService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
