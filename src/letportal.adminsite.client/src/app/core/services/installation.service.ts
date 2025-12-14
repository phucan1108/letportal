import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable, Optional } from '@angular/core';
import { Observable } from 'rxjs';
import { PORTAL_BASE_URL } from './portal.service';

export interface InstallRequest {
  app: string;
  versionNumber?: string | null;
  connectionString: string;
  databaseType: string;
}

export interface InstallationResult {
  success: boolean;
  message?: string;
  installedVersion?: string;
  executedSteps: string[];
  errors: string[];
}

export interface VersionInfo {
  versionNumber: string;
  isInstalled: boolean;
  canUpgradeTo: boolean;
  components: string[];
}

export interface AvailableVersionsResponse {
  app: string;
  currentVersion?: string;
  availableVersions: VersionInfo[];
}

export interface InstalledAppVersion {
  name: string;
  currentVersion: string;
  availableVersion: string;
}

export interface CheckedInstallationResult {
  installed: boolean;
  canUpgrade: boolean;
  installedAppVersions?: InstalledAppVersion[];
}

@Injectable({
  providedIn: 'root'
})
export class InstallationClient {
  private baseUrl: string;

  constructor(
    private http: HttpClient,
    @Optional() @Inject(PORTAL_BASE_URL) baseUrl?: string
  ) {
    this.baseUrl = baseUrl ?? 'https://localhost:5102';
  }

  checkInstallation(): Observable<CheckedInstallationResult> {
    const url = `${this.baseUrl}/api/installation/check`;
    return this.http.get<CheckedInstallationResult>(url);
  }

  getAvailableVersions(app: string): Observable<AvailableVersionsResponse> {
    const url = `${this.baseUrl}/api/installation/versions/${encodeURIComponent(app)}`;
    console.log('Fetching available versions from URL:', url);
    return this.http.get<AvailableVersionsResponse>(url);
  }

  install(request: InstallRequest): Observable<InstallationResult> {
    const url = `${this.baseUrl}/api/installation/install`;
    return this.http.post<InstallationResult>(url, request, {
      headers: new HttpHeaders({
        'Content-Type': 'application/json'
      })
    });
  }
}
