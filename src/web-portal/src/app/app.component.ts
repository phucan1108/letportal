import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  showChatBox = false
  constructor(
    private router: Router
  ){
  }
  ngOnInit(): void {  
    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        if(event.urlAfterRedirects.indexOf('/portal/') >= 0){      
          this.showChatBox = true
        }
        else{
          this.showChatBox = false
        }
      }
    }); 
  }
}
