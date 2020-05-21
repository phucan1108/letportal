import {Injectable} from '@angular/core';

@Injectable()
export class DomHandler {

  public addClass(element: any, className: string): void {
    if (element.classList)
      element.classList.add(className);
    else
      element.className += ' ' + className;
  }

  public removeClass(element: any, className: string): void {
    if (element.classList)
      element.classList.remove(className);
    else
      element.className = element.className.replace(new RegExp('(^|\\b)' + className.split(' ').join('|') + '(\\b|$)', 'gi'), ' ');
  }

  public hasClass(element: any, className: string): boolean {
    if (element.classList)
      return element.classList.contains(className);
    else
      return new RegExp('(^| )' + className + '( |$)', 'gi').test(element.className);
  }

  public find(element: any, selector: string): any[] {
    return element.querySelectorAll(selector);
  }

  public findSingle(element: any, selector: string): any {
    return element.querySelector(selector);
  }

  public invokeElementMethod(element: any, methodName: string, args?: any[]): void {
    (element as any)[methodName].apply(element, args);
  }

}
