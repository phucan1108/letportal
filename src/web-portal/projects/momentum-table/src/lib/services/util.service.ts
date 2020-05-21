import {Injectable} from '@angular/core';

@Injectable()
export class ObjectUtils {
  public equals(obj1: any, obj2: any): boolean {
    return this.equalsByValue(obj1, obj2);
  }

  public equalsByValue(obj1: any, obj2: any): boolean {
    if (obj1 == null && obj2 == null) {
      return true;
    }
    if (obj1 == null || obj2 == null) {
      return false;
    }

    if (obj1 == obj2) {
      delete obj1._$visited;
      return true;
    }

    if (typeof obj1 == 'object' && typeof obj2 == 'object') {
      obj1._$visited = true;
      let flag = true;
      for (let p in obj1) {
        if (p === '_$visited') continue;
        if (obj1.hasOwnProperty(p) !== obj2.hasOwnProperty(p)) {
          flag = false;
        }

        switch (typeof (obj1[p])) {
          case 'object':
            if (obj1[p] && obj1[p]._$visited || !this.equals(obj1[p], obj2[p])) flag = false;
            break;

          case 'function':
            if (typeof (obj2[p]) == 'undefined' || (p != 'compare' && obj1[p].toString() != obj2[p].toString())) flag = false;
            break;

          default:
            if (obj1[p] != obj2[p]) flag = false;
            break;
        }
      }

      for (let p in obj2) {
        if (typeof (obj1[p]) == 'undefined') flag = false;
      }

      delete obj1._$visited;
      return flag;
    }

    return false;
  }
}
