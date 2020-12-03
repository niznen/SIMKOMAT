var mysql = require('mysql');

var con = mysql.createConnection({
    host: "10.200.101.27",
    user: "appuser",
    password: "Ej2014kS@",
    database: "ejtest"
});

con.connect(function (err) {
    if (err) throw err;
    con.query(
        "SELECT * FROM iam_sales_session LIMIT 10",
        function (err, result, fields) {
            if (err) throw err;
            console.log(result);
        }
    );
});