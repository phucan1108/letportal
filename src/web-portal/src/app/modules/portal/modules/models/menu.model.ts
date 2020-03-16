import { Menu } from 'services/portal.service';

export interface MenuNode {
    expandable: boolean;
    name: string;
    level: number;
    id: string;
    extMenu: ExtendedMenu;
    checked: boolean;
}

export interface ExtendedMenu extends Menu{
    level: number
}
