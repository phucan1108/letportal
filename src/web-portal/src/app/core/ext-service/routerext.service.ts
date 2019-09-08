import { Injectable } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';

@Injectable()
export class RouterExtService {
  private previousUrl: string = '';
  private currentUrl: string = '';

  constructor(private router: Router) {
    this.currentUrl = this.router.url;
    router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        if(this.previousUrl.indexOf(this.currentUrl) === -1){
          // Not duplicate url          
          this.previousUrl = this.currentUrl;
        }
        this.currentUrl = event.url;
      }
    });
  }

  public getPreviousUrl() {
    return this.previousUrl;
  }
}