import React from 'react';
import Divider from '@material-ui/core/Divider';
import Drawer from '@material-ui/core/Drawer';
import Hidden from '@material-ui/core/Hidden';

import LockOpenIcon from '@material-ui/icons/LockOpen';
import PersonAddIcon from '@material-ui/icons/PersonAdd';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ViewStreamIcon from '@material-ui/icons/ViewStream';
import RssFeedIcon from '@material-ui/icons/RssFeed';
import PostAddIcon from '@material-ui/icons/PostAdd';

import {
  makeStyles,
  useTheme,
  Theme,
  createStyles,
} from '@material-ui/core/styles';
import { MenuList, MenuItem, Typography } from '@material-ui/core';
import { Link, withRouter, RouteComponentProps } from 'react-router-dom';
import { connect } from 'react-redux';
import {
  Translate as T,
  withLocalize,
  LocalizeContextProps,
} from 'react-localize-redux';

import { AppState } from '../..';

interface Props extends RouteComponentProps<any> {
  isOpen: boolean;
  onDrawerChange: () => void;
  onDrawerClose: () => void;
  drawerWidth: number;
  isLoggedIn: boolean;
  userId: number | undefined;
}

function ResponsiveDrawer(props: Props & LocalizeContextProps) {
  const useStyles = makeStyles((theme: Theme) =>
    createStyles({
      root: {
        display: 'flex',
        zIndex: theme.zIndex.drawer,
      },
      drawer: {
        [theme.breakpoints.up('md')]: {
          width: props.drawerWidth,
          flexShrink: 0,
        },
      },
      menuButton: {
        marginRight: theme.spacing(2),
        [theme.breakpoints.up('sm')]: {
          display: 'none',
        },
      },
      toolbar: theme.mixins.toolbar,
      drawerPaper: {
        width: props.drawerWidth,
      },
      menuItem: {
        paddingTop: '10px',
        paddingBottom: '10px',
      },
    }),
  );
  const classes = useStyles();
  const theme = useTheme();

  const {
    location: { pathname },
    isLoggedIn,
    onDrawerClose,
    onDrawerChange,
  } = props;

  const notLoggedMenu = (
    <MenuList>
      <MenuItem
        className={classes.menuItem}
        component={Link}
        to="/"
        selected={'/' === pathname}
        onClick={onDrawerClose}
      >
        <ListItemIcon>
          <LockOpenIcon fontSize="small" />
        </ListItemIcon>
        <Typography variant="inherit">
          <T id="login" />
        </Typography>
      </MenuItem>
      <Divider variant="middle" />
      <MenuItem
        className={classes.menuItem}
        component={Link}
        to="/Register"
        selected={'/Register' === pathname}
        onClick={onDrawerClose}
      >
        <ListItemIcon>
          <PersonAddIcon fontSize="small" />
        </ListItemIcon>
        <Typography variant="inherit">
          <T id="register" />
        </Typography>
      </MenuItem>
      <Divider variant="middle" />
    </MenuList>
  );

  const followPath = `/Following/?userId=${props.userId ? props.userId : '1'}`;

  const loggedInMenu = (
    <MenuList>
      <MenuItem
        className={classes.menuItem}
        component={Link}
        to="/MyFeed"
        selected={pathname.includes('/MyFeed')}
        onClick={onDrawerClose}
      >
        <ListItemIcon>
          <ViewStreamIcon fontSize="small" />
        </ListItemIcon>
        <Typography variant="inherit">
          <T id="myFeed" />
        </Typography>
      </MenuItem>
      <Divider variant="middle" />
      <MenuItem
        className={classes.menuItem}
        component={Link}
        to="/ManagePost"
        selected={pathname.includes('/ManagePost')}
        onClick={onDrawerClose}
      >
        <ListItemIcon>
          <PostAddIcon fontSize="small" />
        </ListItemIcon>
        <Typography variant="inherit">
          <T id="addPost" />
        </Typography>
      </MenuItem>
      <Divider variant="middle" />
      <MenuItem
        component={Link}
        to={followPath}
        selected={pathname.includes('/Following')}
        onClick={onDrawerClose}
        className={classes.menuItem}
      >
        <ListItemIcon>
          <RssFeedIcon fontSize="small" />
        </ListItemIcon>
        <Typography variant="inherit">
          <T id="following" />
        </Typography>
      </MenuItem>
      <Divider variant="middle" />
    </MenuList>
  );

  const drawer = (
    <div>
      <Hidden smDown={true}>
        <div className={classes.toolbar} />
      </Hidden>
      {isLoggedIn ? loggedInMenu : notLoggedMenu}
    </div>
  );

  return (
    <div className={classes.root}>
      <nav className={classes.drawer} aria-label="mailbox folders">
        <Hidden mdUp={true} implementation="css">
          <Drawer
            variant="temporary"
            anchor={theme.direction === 'rtl' ? 'right' : 'left'}
            open={props.isOpen}
            onClose={onDrawerChange}
            classes={{
              paper: classes.drawerPaper,
            }}
            ModalProps={{
              keepMounted: true, // Better open performance on mobile.
            }}
          >
            {drawer}
          </Drawer>
        </Hidden>
        <Hidden smDown={true} implementation="css">
          {/* Big drawer*/}
          <Drawer
            classes={{
              paper: classes.drawerPaper,
            }}
            variant="permanent"
            open={true}
          >
            {drawer}
          </Drawer>
        </Hidden>
      </nav>
    </div>
  );
}

const mapStateToProps = (state: AppState) => ({
  isLoggedIn: state.user.loggedIn,
  userId: state.user.details?.id,
});

export default connect(mapStateToProps)(
  withLocalize(withRouter(ResponsiveDrawer)),
);
