import React from "react";
import { CsvToHtmlTable } from "react-csv-to-table";

import { sampleData } from "./sample";

export const App = () => {
  return (
    <div className="container">

      <h2> The Data Show below rendered from CSv file.  </h2>

      <br/><br/>
      <CsvToHtmlTable
        data={sampleData}
        csvDelimiter=","
        tableClassName="table table-striped table-hover"
      />


      <h2>Your CSV data looks like below</h2>
      <pre>
      {sampleData}
      </pre>
    </div>
  );
};

export default App;