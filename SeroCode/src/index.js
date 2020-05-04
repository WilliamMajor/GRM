
import ReactDOM from 'react-dom';
import './index.css';
import React from "react";
import App from './App';
import csvToHtmltable from './csvToHtmltable.jsx';
import 'bootstrap/dist/css/bootstrap.css';

export default {
    csvToHtmltable
  }
  
  export { csvToHtmltable };

 
ReactDOM.render(<App />, document.getElementById('root'));