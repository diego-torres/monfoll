DROP TABLE IF EXISTS cat_cliente CASCADE;
CREATE TABLE IF NOT EXISTS cat_cliente (
	id_cliente INTEGER NOT NULL PRIMARY KEY,
	CD_CLIENTE VARCHAR(6),
	NOMBRE_CLIENTE VARCHAR(150),
	RUTA VARCHAR(20),
	DIA_PAGO VARCHAR(50)
);

GRANT ALL ON cat_cliente TO monfoll;

DROP TABLE IF EXISTS LOG_CLIENTE CASCADE;
CREATE TABLE IF NOT EXISTS LOG_CLIENTE (
	ID_LOG_CLIENTE SERIAL PRIMARY KEY,
	id_cliente INTEGER REFERENCES cat_cliente(id_cliente) ON DELETE CASCADE,
	ES_LOCAL BOOLEAN,
	NOTA VARCHAR(250)
);

GRANT ALL ON LOG_CLIENTE TO monfoll;

DROP TABLE IF EXISTS ctrl_cuenta CASCADE;
CREATE TABLE IF NOT EXISTS ctrl_cuenta (
	id_doco INTEGER NOT NULL PRIMARY KEY,
	f_documento DATE,
	f_vencimiento DATE,
	f_cobro DATE,
	id_cliente INTEGER REFERENCES cat_cliente(id_cliente) NOT NULL,
	serie_doco VARCHAR(4),
	folio_doco INTEGER,
	tipo_documento VARCHAR(150),
	tipo_cobro VARCHAR(50),
	facturado MONEY,
	saldo MONEY,
	moneda VARCHAR(50),
	observaciones VARCHAR(50),
	ts_descargado TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
GRANT ALL ON ctrl_cuenta TO monfoll;

DROP TABLE IF EXISTS CTRL_ABONO CASCADE;
CREATE TABLE IF NOT EXISTS CTRL_ABONO(
	ID_ABONO INTEGER NOT NULL PRIMARY KEY,
	id_doco INTEGER NOT NULL REFERENCES ctrl_cuenta(id_doco) ON DELETE CASCADE,
	TIPO_PAGO VARCHAR(150),
	IMPORTE_PAGO MONEY,
	FOLIO INTEGER,
	CONCEPTO VARCHAR(150),
	FECHA_DEPOSITO DATE,
	CUENTA VARCHAR(50)
);
GRANT ALL ON CTRL_ABONO TO monfoll;


DROP TABLE IF EXISTS cat_cobrador CASCADE;
CREATE TABLE IF NOT EXISTS cat_cobrador (
	id_cobrador SERIAL PRIMARY KEY,
	nombre_cobrador VARCHAR(150),
	es_local BOOLEAN
);
GRANT ALL ON cat_cobrador TO monfoll;
GRANT ALL ON cat_cobrador_id_cobrador_seq TO monfoll;

DROP TABLE IF EXISTS LOG_COBRADOR CASCADE;
CREATE TABLE IF NOT EXISTS LOG_COBRADOR (
	ID_LOG_COBRADOR SERIAL PRIMARY KEY,
	id_cobrador INTEGER REFERENCES cat_cobrador(id_cobrador) ON DELETE CASCADE,
	NOTA VARCHAR(250)
);
GRANT ALL ON LOG_COBRADOR TO monfoll;
GRANT ALL ON LOG_COBRADOR_ID_LOG_COBRADOR_SEQ TO monfoll;

DROP TABLE IF EXISTS ctrl_asignacion CASCADE;
CREATE TABLE IF NOT EXISTS ctrl_asignacion (
	id_asignacion SERIAL PRIMARY KEY,
	id_cobrador INTEGER NOT NULL REFERENCES cat_cobrador(id_cobrador) ON DELETE CASCADE,
	id_doco INTEGER NOT NULL REFERENCES ctrl_cuenta(id_doco) ON DELETE CASCADE,
	ts_asignado TIMESTAMP DEFAULT CURRENT_TIMESTAMP
); 
GRANT ALL ON ctrl_asignacion TO monfoll;
GRANT ALL ON ctrl_asignacion_id_asignacion_seq TO monfoll;

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

CREATE OR REPLACE FUNCTION ft_upd_acct() RETURNS trigger AS $ft_upd_acct$
	BEGIN
		-- Follow up record
		INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion)
		VALUES(2, NEW.id_doco, 
		'Registro de documento actualizado:: f_cobro: ' ||
		to_char(NEW.f_cobro, 'DD/MM/YYYY') ||
		'; tipo_cobro: ' || NEW.tipo_cobro ||
		'; obs: ' || NEW.observaciones);
		
		RETURN NULL; -- result is ignored since this is an AFTER trigger
	END;
$ft_upd_acct$ LANGUAGE plpgsql;

CREATE TRIGGER tg_upd_acct
AFTER UPDATE ON ctrl_cuenta
	FOR EACH ROW EXECUTE PROCEDURE ft_upd_acct();

CREATE OR REPLACE FUNCTION ft_assign_acct() RETURNS trigger AS $ft_assign_acct$
	DECLARE
		collector varchar(150);
	BEGIN
		-- Remove any previous assignment
		DELETE FROM ctrl_asignacion
		WHERE id_doco = NEW.id_doco
		AND NOT id_cobrador = NEW.id_cobrador;
		
		-- Retrieve collector name
		SELECT nombre_cobrador INTO collector
		FROM cat_cobrador
		WHERE id_cobrador = NEW.id_cobrador;
		
		-- Follow up record
		INSERT INTO ctrl_seguimiento(id_movimiento, id_doco, descripcion)
		VALUES(3, NEW.id_doco, 
		'Documento asignado a ' || collector);
		
		RETURN NULL; -- result is ignored since this is an AFTER trigger
	END;
$ft_assign_acct$ LANGUAGE plpgsql;

CREATE TRIGGER tg_assign_acct
AFTER INSERT ON ctrl_asignacion
	FOR EACH ROW EXECUTE PROCEDURE ft_assign_acct();