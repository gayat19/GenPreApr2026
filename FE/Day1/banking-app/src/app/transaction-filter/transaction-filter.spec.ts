import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountFilter } from './transaction-filter';

describe('AccountFilter', () => {
  let component: AccountFilter;
  let fixture: ComponentFixture<AccountFilter>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccountFilter],
    }).compileComponents();

    fixture = TestBed.createComponent(AccountFilter);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
