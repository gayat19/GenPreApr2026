import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountHome } from './transaction-home';

describe('AccountHome', () => {
  let component: AccountHome;
  let fixture: ComponentFixture<AccountHome>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccountHome],
    }).compileComponents();

    fixture = TestBed.createComponent(AccountHome);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
