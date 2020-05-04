import React from 'react';
import CSVReader from "react-csv-reader";
import { csvToHtmltable } from 'react-csv-to-table';
import { sampleData } from "./sample";


 
function Dashboard(props) {
 
  // handle click event of logout button
  const handleForce = () => {    
    props.history.push('/Home');
  }
 
  return (
    <div className= "container"> 

      Welcome User!<br /> Upload your file here  <br />
      <CSVReader
      cssClass="react-csv-input"
      label=" "
      onFileLoaded={handleForce}
    />

      <input type="button" onClick={handleForce} value="Upload File " />
    </div>

  
        


  );
}
 
export default Dashboard;