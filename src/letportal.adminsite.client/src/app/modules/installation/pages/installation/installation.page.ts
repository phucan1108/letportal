import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { finalize } from 'rxjs/operators';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import {
  CheckedInstallationResult,
  InstallRequest,
  InstallationClient,
  InstallationResult,
  VersionInfo
} from 'services/installation.service';

@Component({
  selector: 'let-installation',
  templateUrl: './installation.page.html',
  styleUrls: ['./installation.page.scss'],
  standalone: false
})
export class InstallationPage implements OnInit {
  installForm!: FormGroup;
  availableVersions: VersionInfo[] = [];
  versionsLoading = false;
  installing = false;
  installResult?: InstallationResult;
  status?: CheckedInstallationResult;
  statusLoading = false;
  databaseTypes = [
    { value: 'mongodb', labelKey: 'pages.installation.options.db.mongodb' },
    { value: 'sqlserver', labelKey: 'pages.installation.options.db.sqlserver' },
    { value: 'postgresql', labelKey: 'pages.installation.options.db.postgresql' },
    { value: 'mysql', labelKey: 'pages.installation.options.db.mysql' }
  ];

  constructor(
    private fb: FormBuilder,
    private installationClient: InstallationClient,
    private snackBar: MatSnackBar,
    private router: Router,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.installForm = this.fb.group({
      app: ['portal', Validators.required],
      versionNumber: [''],
      connectionString: ['', Validators.required],
      databaseType: ['mongodb', Validators.required]
    });

    this.installForm.get('app')?.valueChanges.subscribe(app => {
      if (app) {
        this.loadVersions(app);
      }
    });

    this.loadVersions(this.installForm.get('app')?.value);
    this.loadStatus();
  }

  loadStatus(): void {
    this.statusLoading = true;
    this.installationClient.checkInstallation()
      .pipe(finalize(() => this.statusLoading = false))
      .subscribe({
        next: status => this.status = status,
        error: () => {
          this.status = undefined;
        }
      });
  }

  loadVersions(app: string): void {
    if (!app) {
      this.availableVersions = [];
      return;
    }

    this.versionsLoading = true;
    this.availableVersions = [];
    this.installationClient.getAvailableVersions(app)
      .pipe(finalize(() => this.versionsLoading = false))
      .subscribe({
        next: response => {
          this.availableVersions = response.availableVersions ?? [];
        },
        error: (err) => {
          console.error('Error loading available versions:', err);
          this.snackBar.open(this.translate.instant('pages.installation.messages.loadVersionsFailed'), undefined, { duration: 3000 });
        }
      });
  }

  reloadVersions(): void {
    const app = this.installForm.get('app')?.value;
    this.loadVersions(app);
  }

  onInstall(): void {
    if (this.installForm.invalid) {
      this.installForm.markAllAsTouched();
      return;
    }

    this.installing = true;
    this.installResult = undefined;
    const formValue = this.installForm.value;
    const payload: InstallRequest = {
      app: formValue.app,
      versionNumber: formValue.versionNumber || null,
      connectionString: formValue.connectionString,
      databaseType: formValue.databaseType
    };

    this.installationClient.install(payload)
      .pipe(finalize(() => this.installing = false))
      .subscribe({
        next: result => {
          this.installResult = result;
          this.snackBar.open(result.message || this.translate.instant('pages.installation.messages.installCompleted'), undefined, { duration: 3500 });
          this.loadStatus();
        },
        error: err => {
          const errorResult = this.getErrorResult(err);
          this.installResult = errorResult;
          this.snackBar.open(errorResult.message || this.translate.instant('pages.installation.messages.installFailed'), undefined, { duration: 4000 });
        }
      });
  }

  goToLogin(): void {
    this.router.navigateByUrl('/');
  }

  private getErrorResult(err: any): InstallationResult {
    const body = err?.error;
    if (body && typeof body === 'object' && 'success' in body) {
      return body as InstallationResult;
    }

    const message = this.extractErrorMessage(err);
    return {
      success: false,
      message,
      executedSteps: [],
      errors: []
    };
  }

  private extractErrorMessage(err: any): string {
    if (err?.error?.message) {
      return err.error.message;
    }

    if (err?.messageContent) {
      return err.messageContent;
    }

    if (err?.message) {
      return err.message;
    }

    return this.translate.instant('pages.installation.messages.unable');
  }
}
