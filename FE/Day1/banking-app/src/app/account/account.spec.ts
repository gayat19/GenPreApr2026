import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { Router } from '@angular/router';
import { vi } from 'vitest';

import { Account } from './account';
import { BankingApiService } from '../services/bankingapi.service';

describe('Account', () => {
	let component: Account;
	let fixture: ComponentFixture<Account>;
	let bankingApiServiceSpy: { getAccountDetails: ReturnType<typeof vi.fn> };
	let routerSpy: { navigate: ReturnType<typeof vi.fn> };

	beforeEach(async () => {
		bankingApiServiceSpy = {
			getAccountDetails: vi.fn(),
		};
		routerSpy = {
			navigate: vi.fn(),
		};

		await TestBed.configureTestingModule({
			imports: [Account],
			providers: [
				{ provide: BankingApiService, useValue: bankingApiServiceSpy as unknown as BankingApiService },
				{ provide: Router, useValue: routerSpy as unknown as Router },
			],
		}).compileComponents();

		fixture = TestBed.createComponent(Account);
		component = fixture.componentInstance;
		fixture.detectChanges();
	});

	afterEach(() => {
		vi.useRealTimers();
	});

	it('should create', () => {
		expect(component).toBeTruthy();
	});

	it('should fetch account details after debounce for a non-empty account number', async () => {
		vi.useFakeTimers();

		const mockAccount = {
			accountNumber: '12345',
			accountHolder: 'John Doe',
			balance: 1000,
		};
		bankingApiServiceSpy.getAccountDetails.mockReturnValue(of(mockAccount));

		component.searchAccountNumber = '12345';
		component.getAccountDetails();

		await vi.advanceTimersByTimeAsync(500);

		expect(bankingApiServiceSpy.getAccountDetails).toHaveBeenCalledWith('12345');
		expect(component.accountDetails).toEqual(mockAccount);
	});

	it('should not call api and should set empty object for blank input', async () => {
		vi.useFakeTimers();

		component.searchAccountNumber = '   ';
		component.getAccountDetails();

		await vi.advanceTimersByTimeAsync(500);

		expect(bankingApiServiceSpy.getAccountDetails).not.toHaveBeenCalled();
		expect(component.accountDetails).toEqual({});
	});

	it('should navigate to transaction page with account number in state', () => {
		component.accountDetails = { accountNumber: 'ACC999' };

		component.handleSendMoneyClick();

		expect(routerSpy.navigate).toHaveBeenCalledWith(['/account/transaction'], {
			state: { accNum: 'ACC999' },
		});
	});

	it('should complete and unsubscribe search subject on destroy', () => {
		const searchSubject = (component as any).searchSubject;
		const completeSpy = vi.spyOn(searchSubject, 'complete');
		const unsubscribeSpy = vi.spyOn(searchSubject, 'unsubscribe');

		component.onDestroy();

		expect(completeSpy).toHaveBeenCalled();
		expect(unsubscribeSpy).toHaveBeenCalled();
	});
});
