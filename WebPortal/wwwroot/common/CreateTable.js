function formatHeader(header) {
    return header
        .replace(/([a-z])([A-Z])/g, '$1 $2')
        .replace(/^./, function (char) {
            return char.toUpperCase();
        })
        .replace(/Id\b/g, 'ID')
        .trim();
}

// Update   createTable function to accept a hiddenColumn argument: (to hide what we want in any table)
function createTable(id, response, hiddenColumn = "") {

    // get headers
    var headers = Object.keys(response.data[0]);


    var formattedHeaders = headers.filter(function (head) {
        return head.toLowerCase() !== hiddenColumn.toLowerCase();
    });



    var header = formattedHeaders.map(function (head) {
        return `<th>${formatHeader(head)}</th>`;
    }).join("")


    var table = `<table id='${id}Table' class='table striped-table'>
                <thead>

                ${header}
                
                <th>Actions</th>
                </thead>
                <tbody>

    `
    response.data.map(function (data) {
        var stringifiedItem = JSON.stringify(data).replace(/"/g, '&quot;');
        table += `<tr  data-row="${stringifiedItem}">`;

        // skip the data of hidden columns also
        headers.map(function (head) {
            if (head.toLowerCase() === hiddenColumn.toLowerCase()) {
                return;
            }

            table += `<td>${data[head]}</td>`;
        });

        table += `<td>
       <a href='#' class='editBtn'> <i class="fa-solid fa-pen ms-3 " style="color: rgb(11, 174, 124);"></i><a/>
       <a href='#'  class='deleteBtn'> <i class="fa-solid fa-trash ms-3 " style="color: rgb(215, 13, 43);"></i><a/>
        <a href='#' class='infoBtn'><i class="fa-solid fa-eye  ms-3" style="color: rgb(115, 131, 159);"></i><a/>
    </td>`;

        table += `</tr>`;
    });

    table += `</tbody></table>`

    return table;
}