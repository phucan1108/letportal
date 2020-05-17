import { Component, OnInit } from '@angular/core';
import { AppsClient } from 'services/portal.service';

@Component({
    selector: 'let-app-installation',
    templateUrl: './app-installation.page.html'
})
export class AppInstallationPage implements OnInit {
    constructor(
        private appsClient: AppsClient
    ) { }

    ngOnInit(): void { }
}
