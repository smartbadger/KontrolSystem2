import { Expression, Node, ValidationError } from ".";
import { RealizedType, TO2Type } from "./to2-type";
import { Operator } from "./operator";
import { InputPosition, InputRange, WithPosition } from "../../parser";
import { BlockContext } from "./context";
import { SemanticToken } from "../../syntax-token";
import { DefinitionRef } from "./definition-ref";

export class VariableAssign extends Expression {
  public reference?: { sourceRange: InputRange; definition: DefinitionRef };

  constructor(
    public readonly name: WithPosition<string>,
    public readonly op: Operator,
    public readonly expression: Expression,
    start: InputPosition,
    end: InputPosition,
  ) {
    super(start, end);
  }

  resultType(context: BlockContext, typeHint?: RealizedType): TO2Type {
    return this.expression.resultType(context, typeHint);
  }

  public reduceNode<T>(
    combine: (previousValue: T, node: Node) => T,
    initialValue: T,
  ): T {
    return this.expression.reduceNode(combine, combine(initialValue, this));
  }

  public validateBlock(
    context: BlockContext,
    typeHint?: RealizedType,
  ): ValidationError[] {
    const errors: ValidationError[] = [];

    const { definition, value: variableType } =
      context.findVariable(
        [this.name.value],
        this.expression
          .resultType(context, typeHint)
          .realizedType(context.module),
      ) ?? {};
    if (definition) {
      this.reference = {
        sourceRange: this.name.range,
        definition,
      };
    }

    if (!variableType) {
      errors.push({
        status: "error",
        message: `Undefined variable: ${this.name.value}`,
        range: this.name.range,
      });
    } else {
      this.documentation = [
        this.name.range.with(
          `Variable \`${this.name.value} : ${variableType.name}\``,
        ),
      ];
    }

    errors.push(
      ...this.expression.validateBlock(
        context,
        variableType?.realizedType(context.module),
      ),
    );

    return errors;
  }

  public collectSemanticTokens(semanticTokens: SemanticToken[]): void {
    semanticTokens.push(
      this.name.range.semanticToken("variable", "modification"),
    );
    this.expression.collectSemanticTokens(semanticTokens);
  }
}
