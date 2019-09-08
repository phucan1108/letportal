import { Component, OnInit, ChangeDetectorRef, ChangeDetectionStrategy } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Observable } from 'rxjs';
import { map, filter, tap } from 'rxjs/operators';
import { Store } from '@ngxs/store';
import { UserSelectAppAction } from 'stores/apps/app.action';
import { Menu } from 'services/portal.service';
import { Router, Event, ActivatedRoute, NavigationStart, NavigationEnd, NavigationCancel, NavigationError } from '@angular/router';
import { SessionService } from 'services/session.service';
import { SecurityService } from 'app/core/security/security.service';
import { AuthUser } from 'app/core/security/auth.model';
import { AppDashboardComponent } from '../app-dashboard/app-dashboard.component';

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class NavigationComponent implements OnInit {

  authUser: AuthUser
  loading = false
  menus: Array<Menu> = [];
  isExpanded = true;
  isHandset$: Observable<boolean> = this.breakpointObserver.observe(Breakpoints.Handset)
    .pipe(
      map(result => result.matches)
    );

  hasMenu: boolean = !!this.menus && this.menus.length > 0
  hasSelectedApp = false
  constructor(
    private breakpointObserver: BreakpointObserver,
    private store: Store,
    private activatedRouter: ActivatedRoute,
    private router: Router,
    private session: SessionService,
    private security: SecurityService,
    private cd: ChangeDetectorRef) {
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
    this.router.events.subscribe((event: Event) => {
      switch (true) {
        case event instanceof NavigationStart:
          this.loading = true
          break        
        case event instanceof NavigationEnd: 
        case event instanceof NavigationCancel:
        case event instanceof NavigationError:
          this.loading = false
          break
        default:
          break
      }
    })
  }

  ngOnInit(): void {
    this.hasSelectedApp = this.session.getCurrentApp() ? true : false
    this.authUser = this.security.getAuthUser()
    // If user stays in App Selector page, we don't display menu
    if (this.activatedRouter.snapshot.firstChild.component !== AppDashboardComponent) {
      this.store.select(a => a.selectedApp).pipe(
        filter(result => result.filterState === UserSelectAppAction),
        tap(result => {
          this.hasSelectedApp = true
          this.menus = result.selectedApp.menus
          this.cd.markForCheck()
        })
      ).subscribe()
      const currentApp = this.session.getCurrentApp();
      if (currentApp) {
        this.menus = currentApp.menus
      }
    }
  }

  logout() {
    this.security.userLogout()
    this.session.clear()
    this.router.navigateByUrl('/login')
  }
}
