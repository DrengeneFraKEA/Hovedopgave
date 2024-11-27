import { TestBed } from '@angular/core/testing';

import { AdminrightsService } from './adminrights.service';

describe('AdminrightsService', () => {
  let service: AdminrightsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AdminrightsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
