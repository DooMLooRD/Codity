import React, { Component } from 'react';
import PrimarySearchAppBar from './components/layout/navbar';
import { BrowserRouter, Switch, Route } from 'react-router-dom';
import {
  MuiThemeProvider,
  createMuiTheme,
  CssBaseline,
  responsiveFontSizes,
} from '@material-ui/core';
import { ThemeOptions } from '@material-ui/core/styles/createMuiTheme';
import { ToastContainer } from 'react-toastify';
import { renderToStaticMarkup } from 'react-dom/server';
import { withLocalize, LocalizeContextProps } from 'react-localize-redux';
import { globalTranslations } from './translations/index';

import 'react-toastify/dist/ReactToastify.css';

import * as authentication from './components/containers/authentication/index';

// import themeDark from './themes/dark-theme';
import themeLight from './themes/light-theme';


// const darkTheme = responsiveFontSizes(createMuiTheme(themeDark as ThemeOptions));
const lightTheme = responsiveFontSizes(createMuiTheme(themeLight as ThemeOptions));

// tslint:disable-next-line: typedef
class App extends Component<LocalizeContextProps> {
  constructor(props: LocalizeContextProps) {
    super(props);
    this.props.initialize({
      languages: [
        { name: 'English', code: 'en' },
        { name: 'Polish', code: 'pl' },
      ],
      translation: globalTranslations,
      options: {
        renderToStaticMarkup,
        renderInnerHtml: true,
        defaultLanguage: 'pl',
      },
    });
  }
  render() {
    // tslint:disable-next-line: no-console
    console.log('This app is in:', process.env.NODE_ENV, 'mode');
    return (
      <MuiThemeProvider theme={lightTheme}>
        <CssBaseline />
        <ToastContainer />
        <BrowserRouter>
          <PrimarySearchAppBar />
          <Switch>
            <Route path="/" exact={true} component={authentication.Login} />
            <Route path="/Login" component={authentication.Login} />
            <Route path="/Register" component={authentication.Register} />
          </Switch>
        </BrowserRouter>
      </MuiThemeProvider>
    );
  }
}

export default withLocalize(App);