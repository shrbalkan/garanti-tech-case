(function($) {

    $.fn.tableCreater = function(options) {
        const defaults = {
            data: [],
            columns: [
                { field: 'name', title: 'Name' },
                { field: 'tckn', title: 'TCKN' },
                { field: 'phone', title: 'Phone' },
            ],
            dataUrl: 'personnelData.json'
        };

        const settings = $.extend({}, defaults, options);

        return this.each(function() {
            const $container = $(this);
            
            async function loadData() {
                try {
                    const response = await fetch(settings.dataUrl);
                    if (!response.ok) {
                        throw new Error(`HTTP error! status: ${response.status}`);
                    }
                    settings.data = await response.json();
                    await createTable();
                } catch (error) {
                    console.error('Error loading data:', error);
                    $container.html('<div class="error">Error loading data</div>');
                }
            }

            // Async tablo oluşturma
            async function createTable() {
                const table = $('<table/>', {
                    'class': 'personnel-table',
                    'id': 'personnelTable'
                });
                
                const thead = await createTableHeader();
                const tbody = $('<tbody/>');
                
                table.append(thead).append(tbody);
                $container.append(table);
                
                await populateTable();
                bindEvents();
            }

            // Async header oluşturma
            async function createTableHeader() {
                return new Promise(resolve => {
                    const thead = $('<thead/>');
                    const tr = $('<tr/>');
                    
                    tr.append($('<th/>'));
                    settings.columns.forEach(column => {
                        tr.append($('<th/>', { text: column.title }));
                    });
                    
                    thead.append(tr);
                    resolve(thead);
                });
            }

            function createPersonRow(person) {
                const row = $('<tr/>', { 'data-id': person.tckn });
                
                row.append($('<td/>').append(
                    $('<i/>', { 'class': 'fas fa-plus expand-btn' })
                ));
                
                settings.columns.forEach(column => {
                    const cell = $('<td/>');
                    if (column.field === 'tckn') {
                        cell.append($('<a/>', {
                            'href': '#',
                            'class': 'ssn-link',
                            'text': person[column.field]
                        }));
                    } else if (column.field === 'phone') {
                        cell.append($('<a/>', {
                            'href': '#',
                            'class': 'phone-link',
                            'text': person[column.field]
                        }));
                    } else {
                        cell.text(person[column.field]);
                    }
                    row.append(cell);
                });
                
                return row;
            }

            function createExpandedRows(person) {
                if (!person || !person.personnels) return [];
                
                return person.personnels.map(subordinate => {
                    const $row = $('<tr/>', { 
                        'class': 'expanded-row',
                        'data-parent': person.tckn
                    });
                    
                    $row.append($('<td/>', {
                        'style': 'padding-left: 40px;'
                    }));
                    
                    settings.columns.forEach(column => {
                        const $cell = $('<td/>');
                        if (column.field === 'tckn') {
                            $cell.append($('<a/>', {
                                'href': '#',
                                'class': 'ssn-link',
                                'text': subordinate[column.field]
                            }));
                        } else if (column.field === 'phone') {
                            $cell.append($('<a/>', {
                                'href': '#',
                                'class': 'phone-link',
                                'text': subordinate[column.field]
                            }));
                        } else {
                            $cell.text(subordinate[column.field]);
                        }
                        $row.append($cell);
                    });
                    
                    return $row;
                });
            }

            // Async tablo doldurma
            async function populateTable() {
                const tbody = $container.find('tbody');
                const chunks = chunkArray(settings.data, 50); // Veriyi 50'lik parçalara böl

                for (const chunk of chunks) {
                    await new Promise(resolve => {
                        setTimeout(() => {
                            chunk.forEach(person => {
                                tbody.append(createPersonRow(person));
                            });
                            resolve();
                        }, 0);
                    });
                }
            }

            // Veriyi parçalara bölme yardımcı fonksiyonu
            function chunkArray(array, size) {
                const chunks = [];
                for (let i = 0; i < array.length; i += size) {
                    chunks.push(array.slice(i, i + size));
                }
                return chunks;
            }

            // Async personel detay yükleme
            async function loadPersonDetails(tckn) {
                try {
                    const person = await findPersonByTckn(tckn);
                    if (person) {
                        $('#ssnDetailsContent').html(`
                            <p><strong>Name:</strong> ${person.name}</p>
                            <p><strong>TCKN:</strong> ${person.tckn}</p>
                            <p><strong>Birth Place:</strong> ${person.birthPlace}</p>
                            <p><strong>Birth Date:</strong> ${person.birthDate}</p>
                        `);
                        $('#ssnDetailsPopup').fadeIn();
                    }
                } catch (error) {
                    console.error('Error loading person details:', error);
                }
            }

            // Async personel bulma
            async function findPersonByTckn(tckn) {
                return new Promise(resolve => {
                    for (const person of settings.data) {
                        if (String(person.tckn) === tckn) {
                            resolve(person);
                            return;
                        }
                        if (person.personnels) {
                            const subordinate = person.personnels.find(p => String(p.tckn) === tckn);
                            if (subordinate) {
                                resolve(subordinate);
                                return;
                            }
                        }
                    }
                    resolve(null);
                });
            }

            function bindEvents() {
                $container.on('click', '.expand-btn', async function(e) {
                    e.preventDefault();
                    e.stopPropagation();
                    
                    const $icon = $(this);
                    const $row = $icon.closest('tr');
                    const tckn = String($row.data('id'));
                    
                    try {
                        const person = await findPersonByTckn(tckn);
                        
                        if ($icon.hasClass('fa-plus') && person && person.personnels) {
                            $icon.removeClass('fa-plus').addClass('fa-minus');
                            const $expandedRows = createExpandedRows(person);
                            $expandedRows.forEach($expandedRow => {
                                $row.after($expandedRow);
                            });
                        } else {
                            $icon.removeClass('fa-minus').addClass('fa-plus');
                            $container.find(`tr[data-parent="${tckn}"]`).remove();
                        }
                    } catch (error) {
                        console.error('Error expanding row:', error);
                    }
                });

                $container.on('click', '.ssn-link', async function(e) {
                    e.preventDefault();
                    const tckn = $(this).text();
                    await loadPersonDetails(tckn);
                });

                $container.on('click', '.phone-link', function(e) {
                    e.preventDefault();
                    const phone = $(this).text();
                    
                    $('#callButton').attr('href', `tel:${phone}`);
                    $('#smsButton').attr('href', `sms:${phone}`);
                    $('#whatsappButton').attr({
                        'href': `https://wa.me/${phone}`,
                        'target': '_blank'
                    });
                    
                    $('#phonePopup').fadeIn();
                });

                $('.close-btn').click(function() {
                    $(this).closest('.popup').fadeOut();
                });

                $(window).click(function(e) {
                    if ($(e.target).hasClass('popup')) {
                        $('.popup').fadeOut();
                    }
                });
            }

            // Initialize
            loadData();
        });
    };

    // Plugin initialization
    $(document).ready(function() {
        const $container = $("#container");
        if ($container.length) {
            $container.tableCreater();
        }
    });

})(jQuery); 