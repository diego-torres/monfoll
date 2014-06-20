DROP TABLE IF EXISTS cat_empresa CASCADE;
CREATE TABLE IF NOT EXISTS cat_empresa
(
	id_empresa INTEGER NOT NULL PRIMARY KEY,
	nombre_empresa VARCHAR(150),
	ruta VARCHAR(253)
);

DROP TABLE IF EXISTS cat_concepto;
CREATE TABLE IF NOT EXISTS cat_concepto
(
	id_concepto SERIAL PRIMARY KEY,
	ap_id INTEGER NOT NULL,
	id_empresa INTEGER NOT NULL REFERENCES cat_empresa(id_empresa) ON DELETE CASCADE,
	codigo_concepto VARCHAR(50),
	nombre_concepto VARCHAR(150),
	razon VARCHAR(50)
);

DROP TABLE IF EXISTS cat_cliente CASCADE;
CREATE TABLE IF NOT EXISTS cat_cliente (
	id_cliente SERIAL PRIMARY KEY,
	ap_id INTEGER NOT NULL,
	id_empresa INTEGER NOT NULL REFERENCES cat_empresa(id_empresa) ON DELETE CASCADE,
	cd_cliente VARCHAR(6),
	nombre_cliente VARCHAR(150),
	ruta VARCHAR(20),
	dia_pago VARCHAR(50),
	es_local BOOLEAN DEFAULT TRUE,
	lista_negra BOOLEAN DEFAULT FALSE
);

GRANT ALL ON cat_cliente TO monfoll;

DROP TABLE IF EXISTS log_cliente CASCADE;
CREATE TABLE IF NOT EXISTS log_cliente (
	id_log_cliente SERIAL PRIMARY KEY,
	id_cliente INTEGER REFERENCES cat_cliente(id_cliente) ON DELETE CASCADE,
	nota VARCHAR(250),
	ts_seguimiento TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

GRANT ALL ON log_cliente TO monfoll;
GRANT ALL ON log_cliente_id_log_cliente_seq TO monfoll;

DROP TABLE IF EXISTS ctrl_cuenta CASCADE;
CREATE TABLE IF NOT EXISTS ctrl_cuenta (
	id_doco SERIAL PRIMARY KEY,
	ap_id INTEGER NOT NULL,
	enterprise_id INTEGER NOT NULL,
	f_documento DATE,
	f_vencimiento DATE,
	f_cobro DATE,
	f_cobro_esperada DATE,
	id_cliente INTEGER REFERENCES cat_cliente(id_cliente) NOT NULL,
	serie_doco VARCHAR(4),
	folio_doco INTEGER,
	tipo_documento VARCHAR(150),
	tipo_cobro VARCHAR(50),
	facturado MONEY,
	saldo MONEY,
	moneda VARCHAR(50),
	observaciones VARCHAR(50),
	lista_negra BOOLEAN DEFAULT FALSE,
	ts_descargado TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
GRANT ALL ON ctrl_cuenta TO monfoll;

DROP TABLE IF EXISTS ctrl_abono CASCADE;
CREATE TABLE IF NOT EXISTS ctrl_abono(
	id_abono INTEGER NOT NULL PRIMARY KEY,
	id_doco INTEGER NOT NULL REFERENCES ctrl_cuenta(id_doco) ON DELETE CASCADE,
	tipo_pago VARCHAR(150),
	importe_pago MONEY,
	folio INTEGER,
	concepto VARCHAR(150),
	fecha_deposito DATE,
	cuenta VARCHAR(50)
);
GRANT ALL ON ctrl_abono TO monfoll;

DROP TABLE IF EXISTS cat_movimiento CASCADE;
CREATE TABLE IF NOT EXISTS cat_movimiento (
	id_movimiento SERIAL PRIMARY KEY,
	descripcion VARCHAR(100),
	system_based BOOLEAN
);
GRANT ALL ON cat_movimiento TO monfoll;
GRANT ALL ON cat_movimiento_id_movimiento_seq TO monfoll;

DROP TABLE IF EXISTS ctrl_seguimiento CASCADE;
CREATE TABLE IF NOT EXISTS ctrl_seguimiento (
	id_seguimiento SERIAL PRIMARY KEY,
	id_movimiento INTEGER NOT NULL REFERENCES CAT_MOVIMIENTO(id_movimiento),
	id_doco INTEGER NOT NULL REFERENCES ctrl_cuenta(id_doco) ON DELETE CASCADE,
	descripcion VARCHAR(250),
	ts_seguimiento TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
GRANT ALL ON ctrl_seguimiento TO monfoll;
GRANT ALL ON ctrl_seguimiento_id_seguimiento_seq TO monfoll;

/*
**************************************************************************************************
                      TRIGGERS AND PROCEDURES
**************************************************************************************************
*/

CREATE OR REPLACE FUNCTION ft_new_acct() RETURNS trigger AS $ft_new_acct$
	BEGIN
		-- Follow up record
		INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion)
		VALUES(1, NEW.id_doco, 'Documento descargado de AdminPaq');
		
		RETURN NULL; -- result is ignored since this is an AFTER trigger
	END;
$ft_new_acct$ LANGUAGE plpgsql;

CREATE TRIGGER tg_new_acct
AFTER INSERT ON ctrl_cuenta
	FOR EACH ROW EXECUTE PROCEDURE ft_new_acct();